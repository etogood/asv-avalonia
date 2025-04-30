using System.Composition;

namespace Asv.Avalonia;

[ExportUnitItem(ProgressBase.Id)]
[Shared]
[method: ImportingConstructor]
public class InPartsProgressUnit() : UnitItemBase(0.01)
{
    public const string Id = $"{ProgressBase.Id}.in.parts";

    public override string UnitItemId => Id;
    public override string Name => RS.InParts_UnitItem_Name;
    public override string Description => RS.InParts_Progress_Description;
    public override string Symbol => string.Empty;
    public override bool IsInternationalSystemUnit => false;
}
