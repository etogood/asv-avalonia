using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Input;
using Material.Icons;

namespace Asv.Avalonia
{
    public interface IMenuItem : IViewModel
    {
        string ParentId { get; }
        int Order { get; }
        MaterialIconKind Icon { get; }
        string Header { get; }
        ICommand Command { get; }
        object? CommandParameter { get; }
        bool IsVisible { get; }
        bool StaysOpenOnClick { get; }
        public bool IsEnabled { get; }
        public KeyGesture? HotKey { get; }
    }
}
