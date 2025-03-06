using ObservableCollections;

namespace Asv.Avalonia;

public interface IHomePage : IPage
{
    ObservableList<IActionViewModel> Tools { get; }
    ObservableList<IHomePageItem> Items { get; }
}

public interface IHomePageItem : IHeadlinedViewModel
{
    ObservableList<IActionViewModel> Actions { get; }
    ObservableList<IHeadlinedViewModel> Info { get; }
}

public class HomePageItem(string id) : HeadlinedViewModel(id), IHomePageItem
{
    public ObservableList<IActionViewModel> Actions { get; } = new();
    public ObservableList<IHeadlinedViewModel> Info { get; } = new();
}
