using Asv.Common;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using R3;

namespace Asv.Avalonia;

public interface ISettingsPage : IPage
{
    ObservableList<ITreePage> Nodes { get; }
}

public interface ISettingsSubPage : ITreeSubpage<ISettingsPage> { }

public abstract class SettingsSubPage(NavigationId id, ILoggerFactory loggerFactory)
    : TreeSubpage<ISettingsPage>(id, loggerFactory),
        ISettingsSubPage
{
    public override ValueTask Init(ISettingsPage context) => ValueTask.CompletedTask;

    public override IEnumerable<IRoutable> GetRoutableChildren() => Menu;
}
