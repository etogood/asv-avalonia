using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Asv.Mavlink;
using R3;

namespace Asv.Avalonia.Example;

public class BurstDownloadDialogViewModel(string id) : DialogViewModelBase(id)
{
    [Range(1, MavlinkFtpHelper.MaxDataSize)]
    public BindableReactiveProperty<byte?> PacketSize { get; set; } =
        new BindableReactiveProperty<byte?>(MavlinkFtpHelper.MaxDataSize).EnableValidation();

    public byte DialogResult { get; set; }

    public void ApplyDialog(ContentDialog dialog)
    {
        dialog.PrimaryButtonCommand = IsValid.ToReactiveCommand(_ =>
            DialogResult = PacketSize.Value ?? MavlinkFtpHelper.MaxDataSize
        );
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PacketSize.Dispose();
        }

        base.Dispose(disposing);
    }
}
