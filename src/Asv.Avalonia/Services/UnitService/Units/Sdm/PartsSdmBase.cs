using System.Composition;

namespace Asv.Avalonia;

[ExportUnitItem(SdmBase.Id)]
[Shared]
[method: ImportingConstructor]
public sealed class PartsSdmBase() : UnitItemBase(1)
{
    public const string Id = $"{SdmBase.Id}.parts";

    public override string UnitItemId => Id;
    public override string Name => "In Parts"; // TODO: Localize
    public override string Description => "SDM in parts"; // TODO: Localize
    public override string Symbol => string.Empty;
    public override bool IsInternationalSystemUnit => false;
}
