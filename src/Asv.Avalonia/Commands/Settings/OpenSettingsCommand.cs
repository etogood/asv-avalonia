using System.Composition;
using Avalonia.Input;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[method: ImportingConstructor]
public class OpenSettingsCommand(INavigationService nav)
    : OpenPageCommandBase(SettingsPageViewModel.PageId, nav)
{
    #region Static

    public const string Id = $"{BaseId}.open.{SettingsPageViewModel.PageId}";

    public static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.OpenSettingsCommand_CommandInfo_Name,
        Description = RS.OpenSettingsCommand_CommandInfo_Description,
        Icon = SettingsPageViewModel.PageIcon,
        HotKeyInfo = new HotKeyInfo { DefaultHotKey = KeyGesture.Parse("Ctrl+S") },
        Source = SystemModule.Instance,
    };

    #endregion
    public override ICommandInfo Info => StaticInfo;
}
