namespace Asv.Avalonia;

public class NullUnitService : IUnitService
{
    #region Static

    public static NullUnitService Instance { get; } = new();

    #endregion

    private NullUnitService()
        : this(new NullUnitBase([new NullUnitItemInternational(), new NullUnitItem()])) { }

    private NullUnitService(NullUnitBase nullUnitBase)
    {
        Units = new DesignTimeDictionary<string, IUnit>(
            new KeyValuePair<string, IUnit>(NullUnitBase.Id, nullUnitBase)
        );
    }

    public IReadOnlyDictionary<string, IUnit> Units { get; }
}
