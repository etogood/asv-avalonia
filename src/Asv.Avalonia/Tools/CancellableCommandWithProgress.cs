using System.Runtime.CompilerServices;
using System.Windows.Input;
using Asv.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using R3;
using ZLogger;

namespace Asv.Avalonia;

public static class CoreDesignTime
{
    public static CancellableCommandWithProgress CancellableCommand() =>
        new((_, _) => Task.CompletedTask, "Default", NullLoggerFactory.Instance);
}

public delegate Task CommandExecuteDelegate(IProgress<double> progress, CancellationToken cancel);

public class CancellableCommandWithProgress : AsyncDisposableOnce, IProgress<double>
{
    private readonly CommandExecuteDelegate _execute;
    private readonly ReactiveCommand _cancelCommand;
    private readonly ReactiveCommand _command;
    private readonly ILogger<CancellableCommandWithProgress> _logger;
    private CancellationTokenSource? _cancellationTokenSource;

    public CancellableCommandWithProgress(
        CommandExecuteDelegate execute,
        string title,
        ILoggerFactory logFactory
    )
    {
        _logger = logFactory.CreateLogger<CancellableCommandWithProgress>();

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
        }

        _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        Title = new BindableReactiveProperty<string>(title);
        IsExecuting = new BindableReactiveProperty<bool>();
        CanExecute = new BindableReactiveProperty<bool>(true);
        Progress = new BindableReactiveProperty<double>();

        _command = new ReactiveCommand(_ => InternalExecute().SafeFireAndForget());
        _cancelCommand = new ReactiveCommand(InternalCancel);
    }

    private void InternalCancel(Unit obj)
    {
        _cancellationTokenSource?.Cancel(false);
        _logger.LogWarning($"Command '{Title}' was cancelled");
        IsExecuting.Value = false;
        CanExecute.Value = true;
        Progress.Value = 1.0;
    }

    private async Task InternalExecute()
    {
        try
        {
            _cancellationTokenSource?.Cancel(false);
            _cancellationTokenSource = new CancellationTokenSource();
            IsExecuting.Value = true;
            CanExecute.Value = false;
            Progress.Value = 0;
            _logger.ZLogTrace($"Start command '{Title}'");
            await _execute(this, _cancellationTokenSource.Token);
            _logger.ZLogTrace($"Command '{Title}' completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error to execute command {Title}:{ex.Message}");
        }
        finally
        {
            IsExecuting.Value = false;
            CanExecute.Value = true;
            Progress.Value = 1;
        }
    }

    public BindableReactiveProperty<string> Title { get; }
    public BindableReactiveProperty<bool> IsExecuting { get; }
    public BindableReactiveProperty<bool> CanExecute { get; }
    public BindableReactiveProperty<double> Progress { get; }

    public ICommand Cancel => _cancelCommand;
    public ICommand Command => _command;

    public void Report(double value) => Progress.Value = value;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cancelCommand.Dispose();
            _command.Dispose();
            _cancellationTokenSource?.Dispose();
            Title.Dispose();
            IsExecuting.Dispose();
            CanExecute.Dispose();
            Progress.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await CastAndDispose(_cancelCommand);
        await CastAndDispose(_command);
        if (_cancellationTokenSource != null)
        {
            await CastAndDispose(_cancellationTokenSource);
        }

        await CastAndDispose(Title);
        await CastAndDispose(IsExecuting);
        await CastAndDispose(CanExecute);
        await CastAndDispose(Progress);

        await base.DisposeAsyncCore();

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync();
            }
            else
            {
                resource.Dispose();
            }
        }
    }
}
