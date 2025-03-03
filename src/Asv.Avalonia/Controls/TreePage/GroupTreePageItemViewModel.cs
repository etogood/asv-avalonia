namespace Asv.Avalonia;

public class GroupTreePageItemViewModel : RoutableViewModel
{
    public GroupTreePageItemViewModel(string id)
        : base(id) { }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }
}
