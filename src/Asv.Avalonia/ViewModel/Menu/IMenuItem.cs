using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Input;
using Material.Icons;

namespace Asv.Avalonia;

public interface IMenuItem : IActionViewModel
{
    NavigationId ParentId { get; }
    bool StaysOpenOnClick { get; }
    public bool IsEnabled { get; }
    public KeyGesture? HotKey { get; }
}

public static class MenuItemMixin
{
    public static IMenuItem CreateMenu(this ICommandInfo cmdInfo)
    {
        var item = new MenuItem(cmdInfo.Id, cmdInfo.Name)
        {
            Description = cmdInfo.Description,
            Icon = cmdInfo.Icon,
        };
        item.Command = cmdInfo.CreateSystemCommand(item);
        return item;
    }
}
