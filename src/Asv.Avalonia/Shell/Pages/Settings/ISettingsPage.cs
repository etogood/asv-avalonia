using Asv.Common;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

public interface ISettingsPage : IPage
{
    ObservableList<ITreePage> Nodes { get; }
}

public interface ISettingsSubPage : ITreeSubpage<ISettingsPage> { }

public abstract class SettingsSubPage(NavigationId id)
    : TreeSubpage<ISettingsPage>(id),
        ISettingsSubPage
{
    public override ValueTask Init(ISettingsPage context) => ValueTask.CompletedTask;

    public override IEnumerable<IRoutable> GetRoutableChildren() => Menu;
}
