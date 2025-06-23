using Material.Icons;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

public enum RttBoxStatus
{
    Normal,
    Unknown,
    Success,
    Warning,
    Error,
    Info1,
    Info2,
    Info3,
    Info4,
}

public class RttBoxViewModel : RoutableViewModel
{
    public RttBoxViewModel()
        : base(DesignTime.Id, DesignTime.LoggerFactory)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    public RttBoxViewModel(NavigationId id, ILoggerFactory loggerFactory)
        : base(id, loggerFactory) { }

    public bool? IsUpdated
    {
        get;
        set => SetField(ref field, value);
    }

    public MaterialIconKind? Icon
    {
        get;
        set => SetField(ref field, value);
    }

    public string? Header
    {
        get;
        set => SetField(ref field, value);
    }

    public string? ShortHeader
    {
        get;
        set => SetField(ref field, value);
    }

    public RttBoxStatus Status
    {
        get;
        set => SetField(ref field, value);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }
}
