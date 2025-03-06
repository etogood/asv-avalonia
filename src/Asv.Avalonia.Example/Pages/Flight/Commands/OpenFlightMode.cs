using System.Composition;
using Avalonia.Input;

namespace Asv.Avalonia.Example.Commands;

[ExportCommand]
[method: ImportingConstructor]
public class OpenFlightModeCommand() : OpenPageCommandBase(FlightPageViewModel.PageId)
{
    #region Static

    public const string Id = $"{BaseId}.open.flight";

    public static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = "Flight mode",
        Description = "Open flight mode map",
        Icon = FlightPageViewModel.PageIcon,
        DefaultHotKey = KeyGesture.Parse("Ctrl+F2"),
        Source = SystemModule.Instance,
    };

    #endregion
    public override ICommandInfo Info => StaticInfo;
}
