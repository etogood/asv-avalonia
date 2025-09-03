using System.Composition;
using Asv.Avalonia.FileAssociation;
using Material.Icons;

namespace Asv.Avalonia;

[ExportCommand]
[Shared]
[method: ImportingConstructor]
public class OpenFileCommand(IFileAssociationService svc, IDialogService dialogs, IAppPath path)
    : StatelessCommand
{
    #region Static

    public const string Id = $"{BaseId}.file.open";

    public static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = "Open File",
        Description = "Open file",
        Icon = MaterialIconKind.File,
        DefaultHotKey = "Ctrl+O",
        Source = SystemModule.Instance,
    };

    #endregion

    public override ICommandInfo Info => StaticInfo;

    protected override async ValueTask<CommandArg?> InternalExecute(
        CommandArg newValue,
        CancellationToken cancel
    )
    {
        var dlg = dialogs.GetDialogPrefab<OpenFileDialogDesktopPrefab>();
        var result = await dlg.ShowDialogAsync(
            new OpenFileDialogPayload
            {
                Title = "Select file",
                TypeFilter = string.Join(",", svc.OpenFileTypeFilters),
                InitialDirectory = path.UserDataFolder,
            }
        );
        if (result != null)
        {
            svc.OpenFile(result);
        }
        return null;
    }
}
