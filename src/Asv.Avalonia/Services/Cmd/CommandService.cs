using System.Collections.Immutable;
using System.Composition;
using System.Reactive.Disposables;
using Asv.Cfg;
using Asv.Common;
using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Extensions.Logging;
using R3;
using ZLogger;
using Disposable = R3.Disposable;

namespace Asv.Avalonia;

public class CommandServiceConfig
{
    public Dictionary<string, string?> HotKeys { get; set; } = new();
}

[Export(typeof(ICommandService))]
[Shared]
public class CommandService : AsyncDisposableOnce, ICommandService
{
    private readonly INavigationService _nav;
    private readonly IConfiguration _cfg;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ImmutableDictionary<string, ICommandFactory> _commands;
    private ImmutableDictionary<string, KeyGesture> _commandsVsGesture;
    private ImmutableDictionary<KeyGesture, ICommandFactory> _gestureVsCommand;
    private readonly ILogger<CommandService> _logger;
    private readonly IDisposable _disposeId;
    private readonly Subject<CommandEventArgs> _onCommand;

    [ImportingConstructor]
    public CommandService(
        INavigationService nav,
        IConfiguration cfg,
        [ImportMany] IEnumerable<ICommandFactory> factories,
        ILoggerFactory loggerFactory
    )
    {
        var dispose = Disposable.CreateBuilder();

        _nav = nav;
        _cfg = cfg;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<CommandService>();
        _commands = factories.ToImmutableDictionary(x => x.Info.Id);
        ReloadHotKeys();

        // global event handlers for key events
        InputElement
            .KeyDownEvent.AddClassHandler<TopLevel>(OnKeyDownHandler, handledEventsToo: true)
            .AddTo(ref dispose);

        _onCommand = new Subject<CommandEventArgs>().AddTo(ref dispose);

        _disposeId = dispose.Build();
    }

    private async void OnKeyDownHandler(TopLevel source, KeyEventArgs keyEventArgs)
    {
        try
        {
            if (keyEventArgs.KeyModifiers == KeyModifiers.None)
            {
                // we don't want to handle key events without modifiers
                return;
            }

            if (_nav.SelectedControl.CurrentValue == null)
            {
                return;
            }

            var gesture = new KeyGesture(keyEventArgs.Key, keyEventArgs.KeyModifiers);
            var commandFactory = _gestureVsCommand.GetValueOrDefault(gesture);
            if (commandFactory == null)
            {
                return;
            }

            // TODO: check if we need to request params through the QuickPick dialog
            await InternalExecute(
                commandFactory,
                _nav.SelectedControl.CurrentValue,
                null,
                CancellationToken.None
            );
        }
        catch (Exception e)
        {
            _logger.ZLogError(
                e,
                $"Error on key down [{keyEventArgs.KeyModifiers:F} + {keyEventArgs.Key:G}] handler : {e.Message}"
            );
        }
    }

    private async ValueTask InternalExecute(
        ICommandFactory factory,
        IRoutable context,
        IPersistable? param,
        CancellationToken cancel
    )
    {
        if (factory.CanExecute(context, param))
        {
            var backup = await factory.Execute(context, param, cancel);
            var snapShot = new CommandSnapshot(
                factory.Info.Id,
                context.GetPathToRoot(),
                param,
                backup
            );
            _onCommand.OnNext(new CommandEventArgs(context, factory, snapShot));
            _logger.ZLogTrace($"Execute command {backup}: context: {context}, param: {param}");
        }
    }

    private void ReloadHotKeys(Action<IDictionary<string, string?>>? modifyConfig = null)
    {
        var keyVsCommandBuilder = ImmutableDictionary.CreateBuilder<KeyGesture, ICommandFactory>();
        var commandVsKeyBuilder = ImmutableDictionary.CreateBuilder<string, KeyGesture>();

        // load default hot keys
        foreach (var value in _commands.Values)
        {
            if (value.Info.DefaultHotKey == null)
            {
                // skip commands without hot keys
                continue;
            }

            keyVsCommandBuilder.Add(value.Info.DefaultHotKey, value);
            commandVsKeyBuilder.Add(value.Info.Id, value.Info.DefaultHotKey);
        }

        var config = _cfg.Get<CommandServiceConfig>();
        var configChanged = false;
        if (modifyConfig != null)
        {
            modifyConfig(config.HotKeys);
            configChanged = true;
        }

        // load custom hot keys from config
        foreach (var (commandId, hotKey) in config.HotKeys)
        {
            if (string.IsNullOrWhiteSpace(hotKey))
            {
                if (keyVsCommandBuilder.Remove(commandVsKeyBuilder[commandId]))
                {
                    commandVsKeyBuilder.Remove(commandId);
                }

                continue;
            }

            KeyGesture keyGesture;
            try
            {
                keyGesture = KeyGesture.Parse(hotKey);
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "Invalid hot key {hotKey} for command {commandId} at config",
                    hotKey,
                    commandId
                );
                config.HotKeys.Remove(commandId);
                configChanged = true;
                continue;
            }

            if (_commands.TryGetValue(commandId, out var command) == false)
            {
                _logger.LogWarning(
                    "Command {commandId} not found => remove it from config",
                    commandId
                );
                config.HotKeys.Remove(commandId);
                configChanged = true;
                continue;
            }

            if (command.Info.DefaultHotKey == keyGesture)
            {
                _logger.LogWarning(
                    "Hot key {hotKey} for command {commandId} is default => remove it from config",
                    hotKey,
                    commandId
                );
                config.HotKeys.Remove(commandId);
                configChanged = true;
                continue;
            }

            commandVsKeyBuilder[commandId] = keyGesture;
            keyVsCommandBuilder[keyGesture] = command;
        }

        _gestureVsCommand = keyVsCommandBuilder.ToImmutable();
        _commandsVsGesture = commandVsKeyBuilder.ToImmutable();

        if (configChanged)
        {
            _cfg.Set(config);
        }
    }

    public IEnumerable<ICommandInfo> Commands => _commands.Values.Select(x => x.Info);

    public ICommandHistory CreateHistory(IRoutable? owner)
    {
        var history = new CommandHistory(owner, this, _loggerFactory);
        return history;
    }

    public ValueTask Execute(
        string commandId,
        IRoutable context,
        IPersistable? param = null,
        CancellationToken cancel = default
    )
    {
        if (_commands.TryGetValue(commandId, out var factory))
        {
            return InternalExecute(factory, context, param, cancel);
        }

        throw new CommandException($"Command {commandId} not found");
    }

    public KeyGesture? this[string commandId]
    {
        get => _commandsVsGesture.GetValueOrDefault(commandId);
        set => ReloadHotKeys(config => config[commandId] = value?.ToString());
    }

    public Observable<CommandEventArgs> OnCommand => _onCommand;

    public async ValueTask Undo(CommandSnapshot command, CancellationToken cancel)
    {
        var context = await _nav.GoTo(command.ContextPath);
        if (
            _commands.TryGetValue(command.CommandId, out var factory)
            && command.UndoParameter != null
        )
        {
            await factory.Undo(context, command.Parameter, cancel);
            _logger.ZLogTrace(
                $"Undo command {factory.Info.Id}: context: {context}, param: {command.Parameter}"
            );
        }
    }

    public async ValueTask Redo(CommandSnapshot command, CancellationToken cancel = default)
    {
        var context = await _nav.GoTo(command.ContextPath);
        if (
            _commands.TryGetValue(command.CommandId, out var factory)
            && command.UndoParameter != null
        )
        {
            await factory.Execute(context, command.Parameter, cancel);
            _logger.ZLogTrace(
                $"Redo command {factory.Info.Id}: context: {context}, param: {command.Parameter}"
            );
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposeId.Dispose();
        }

        base.Dispose(disposing);
    }
}
