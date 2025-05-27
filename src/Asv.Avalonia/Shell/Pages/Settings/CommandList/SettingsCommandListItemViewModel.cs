using System.Windows.Input;
using Asv.Common;
using Avalonia.Input;
using R3;

namespace Asv.Avalonia;

public class SettingsCommandListItemViewModel : RoutableViewModel
{
    private readonly ICommandService _commandService;

    public SettingsCommandListItemViewModel(ICommandInfo commandInfo, ICommandService svc)
        : base(commandInfo.Id)
    {
        Info = commandInfo;
        _commandService = svc;
        IsReset = new ReactiveProperty<bool>(false).DisposeItWith(Disposable);
        CurrentHotKeyString = new BindableReactiveProperty<string?>().DisposeItWith(Disposable);
        IsChangingHotKey = new BindableReactiveProperty<bool>(false).DisposeItWith(Disposable);
        IsSelected = new BindableReactiveProperty<bool>(false).DisposeItWith(Disposable);
        IsValid = new BindableReactiveProperty<bool>(false).DisposeItWith(Disposable);

        IsReset
            .Subscribe(reset =>
            {
                if (!reset)
                {
                    return;
                }

                Info.HotKeyInfo.CustomHotKey.Value = null;
                if (Info.HotKeyInfo.DefaultHotKey is not null)
                {
                    svc.SetHotKey(Info.Id, Info.HotKeyInfo.DefaultHotKey);
                }

                CurrentHotKeyString.Value = null;
                IsReset.Value = false;
            })
            .DisposeItWith(Disposable);
        CurrentHotKeyString
            .EnableValidation(
                s =>
                {
                    if (s is null)
                    {
                        return ValidationResult.Success;
                    }

                    try
                    {
                        var hotkey = KeyGesture.Parse(s.TrimEnd('+'));

                        var isExists = _commandService.Commands.Any(cmd =>
                        {
                            if (cmd.HotKeyInfo.CustomHotKey.Value is not null)
                            {
                                return hotkey.Equals(cmd.HotKeyInfo.CustomHotKey.Value);
                            }

                            return hotkey.Equals(cmd.HotKeyInfo.DefaultHotKey);
                        });

                        if (isExists)
                        {
                            IsValid.Value = false;
                            return ValueTask.FromException<ValidationResult>(
                                new HotkeyAlreadyExists(hotkey.ToString())
                            );
                        }

                        IsValid.Value = true;
                        return ValidationResult.Success;
                    }
                    catch (Exception e)
                    {
                        IsValid.Value = false;
                        return ValueTask.FromException<ValidationResult>(e);
                    }
                },
                this,
                true
            )
            .DisposeItWith(Disposable);
        ChangeHotKeyCommand = IsSelected
            .ToReactiveCommand(_ =>
            {
                if (commandInfo.HotKeyInfo.DefaultHotKey is null)
                {
                    return;
                }

                CurrentHotKeyString.Value = null;
                IsChangingHotKey.Value = true;
            })
            .DisposeItWith(Disposable);
        CancelChangeHotKeyCommand = new ReactiveCommand(_ =>
        {
            CurrentHotKeyString.Value = Info.HotKeyInfo.CustomHotKey.Value?.ToString();
            IsChangingHotKey.Value = false;
        }).DisposeItWith(Disposable);
        ConfirmChangeHotKey = new ReactiveCommand(
            async (_, _) =>
            {
                var value = new StringCommandArg(CurrentHotKeyString.Value ?? string.Empty);
                await this.ExecuteCommand(ConfirmChangeHotKeyCommand.Id, value);
            }
        ).DisposeItWith(Disposable);
        IsSelected
            .Subscribe(isSelected =>
            {
                if (isSelected)
                {
                    return;
                }

                if (!IsChangingHotKey.Value)
                {
                    return;
                }

                CancelChangeHotKeyCommand.Execute(Unit.Default);
            })
            .DisposeItWith(Disposable);
    }

    internal void ConfirmChangeHotKeyImpl()
    {
        if (string.IsNullOrEmpty(CurrentHotKeyString.Value))
        {
            IsReset.Value = true;
            return;
        }

        var key = KeyGesture.Parse(CurrentHotKeyString.Value.TrimEnd('+'));
        _commandService.SetHotKey(Info.Id, key);
        Info.HotKeyInfo.CustomHotKey.Value = key;
        IsChangingHotKey.Value = false;
    }

    public ICommandInfo Info { get; }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    public bool Filter(string text)
    {
        return Info.HotKeyInfo.CustomHotKey.Value?.ToString()
                .Contains(text, StringComparison.OrdinalIgnoreCase) == true
            || Info.HotKeyInfo.DefaultHotKey?.ToString()
                .Contains(text, StringComparison.OrdinalIgnoreCase) == true
            || Info.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
            || Info.Source.ModuleName.Contains(text, StringComparison.OrdinalIgnoreCase)
            || Info.Description.Contains(text, StringComparison.OrdinalIgnoreCase);
    }

    public ReactiveProperty<bool> IsReset { get; }
    public BindableReactiveProperty<string?> CurrentHotKeyString { get; }
    public BindableReactiveProperty<bool> IsChangingHotKey { get; }
    public BindableReactiveProperty<bool> IsValid { get; }
    public BindableReactiveProperty<bool> IsSelected { get; }
    public ReactiveCommand ChangeHotKeyCommand { get; set; }
    public ReactiveCommand CancelChangeHotKeyCommand { get; set; }
    public ICommand ConfirmChangeHotKey { get; set; }
}
