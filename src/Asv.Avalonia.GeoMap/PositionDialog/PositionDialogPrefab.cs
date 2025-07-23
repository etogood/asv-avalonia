using System.Composition;
using Asv.Avalonia.GeoMap;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia.GeoMap;

/// <summary>
/// Payload for ShowInputDialog prefab.
/// </summary>
public sealed class PositionDialogPayload
{
    /// <summary>
    /// Gets or inits the title of the dialog.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or inits the message displayed in the dialog.
    /// </summary>
    public required string Message { get; init; }
}

/// <summary>
/// Dialog for entering user's string.
/// </summary>
[ExportDialogPrefab]
[Shared]
[method: ImportingConstructor]
public sealed class PositionDialogPrefab(INavigationService nav, ILoggerFactory loggerFactory)
    : IDialogPrefab<PositionDialogPayload, string?>
{
    public async Task<string?> ShowDialogAsync(PositionDialogPayload dialogPayload)
    {
        using var vm = new PositionDialogViewModel(loggerFactory)
        {
            // Message = dialogPayload.Message,
        };

        var dialogContent = new ContentDialog(vm, nav)
        {
            Title = dialogPayload.Title,
            PrimaryButtonText = RS.DialogButton_Yes,
            SecondaryButtonText = RS.DialogButton_No,
            DefaultButton = ContentDialogButton.Primary,
        };

        var result = await dialogContent.ShowAsync();

        if (result == ContentDialogResult.Primary) { }

        return null;
    }
}
