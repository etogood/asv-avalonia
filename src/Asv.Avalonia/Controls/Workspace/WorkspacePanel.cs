using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Asv.Avalonia;

public class WorkspacePanel : Panel
{
    private readonly StackPanel _leftPanel;
    private readonly StackPanel _rightPanel;
    private readonly Grid _mainGrid;
    private readonly TabControl _bottomTab;
    private readonly AvaloniaList<Control> _bottomPanel;

    static WorkspacePanel()
    {
        AffectsParentArrange<WorkspacePanel>(DockProperty);
    }

    public WorkspacePanel()
    {
        // Создаем основной Grid
        _mainGrid = new Grid { Name = "MainGrid", ShowGridLines = false };
        _mainGrid.ColumnDefinitions =
        [
            new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
            new ColumnDefinition(new GridLength(5, GridUnitType.Pixel)) { MaxWidth = 5 },
            new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
            new ColumnDefinition(new GridLength(5, GridUnitType.Pixel)) { MaxWidth = 5 },
            new ColumnDefinition(new GridLength(1, GridUnitType.Star)),
        ];
        _mainGrid.RowDefinitions =
        [
            new RowDefinition(GridLength.Auto),
            new RowDefinition(new GridLength(3, GridUnitType.Star)),
            new RowDefinition(new GridLength(5, GridUnitType.Pixel)) { MaxHeight = 5 },
            new RowDefinition(new GridLength(1, GridUnitType.Star)),
        ];

        // Левый ScrollViewer с StackPanel
        _leftPanel = new StackPanel { Name = "PART_LeftPanel", Spacing = 8 };
        var leftScrollViewer = new ScrollViewer
        {
            Margin = new Thickness(8, 8, 0, 8),
            Background = null,
            Content = _leftPanel,
        };
        Grid.SetRow(leftScrollViewer, 0);
        Grid.SetColumn(leftScrollViewer, 0);
        Grid.SetRowSpan(leftScrollViewer, 4);

        // Правый ScrollViewer с StackPanel
        _rightPanel = new StackPanel { Name = "PART_RightPanel", Spacing = 8 };
        var rightScrollViewer = new ScrollViewer
        {
            Margin = new Thickness(0, 8, 8, 8),
            Background = null,
            Content = _rightPanel,
        };
        Grid.SetRow(rightScrollViewer, 0);
        Grid.SetColumn(rightScrollViewer, 4);
        Grid.SetRowSpan(rightScrollViewer, 4);

        // Нижний TabControl
        _bottomTab = new TabControl
        {
            Name = "PART_BottomTab",
            TabStripPlacement = Dock.Bottom,
            Background = null,
        };
        Grid.SetRow(_bottomTab, 3);
        Grid.SetColumn(_bottomTab, 2);
        _bottomTab.ItemsSource = _bottomPanel = new AvaloniaList<Control>();

        // Вертикальный GridSplitter между столбцами 0 и 2
        var verticalSplitter1 = new GridSplitter
        {
            Width = 5,
            Background = Brushes.Transparent,
            IsHitTestVisible = true,
            ResizeBehavior = GridResizeBehavior.PreviousAndNext,
        };
        Grid.SetRow(verticalSplitter1, 0);
        Grid.SetRowSpan(verticalSplitter1, 4);
        Grid.SetColumn(verticalSplitter1, 1);

        // Вертикальный GridSplitter между столбцами 2 и 4
        var verticalSplitter2 = new GridSplitter
        {
            Width = 5,
            Background = Brushes.Transparent,
            IsHitTestVisible = true,
            Opacity = 1,
            ResizeBehavior = GridResizeBehavior.PreviousAndNext,
        };
        Grid.SetRow(verticalSplitter2, 0);
        Grid.SetRowSpan(verticalSplitter2, 4);
        Grid.SetColumn(verticalSplitter2, 3);

        // Горизонтальный GridSplitter в строке 2
        var horizontalSplitter = new GridSplitter
        {
            Height = 5,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = Brushes.Transparent,
            IsHitTestVisible = true,
            ResizeBehavior = GridResizeBehavior.PreviousAndNext,
        };
        Grid.SetRow(horizontalSplitter, 2);
        Grid.SetColumn(horizontalSplitter, 1);
        Grid.SetColumnSpan(horizontalSplitter, 3);

        // Добавляем все элементы в Grid
        _mainGrid.Children.Add(leftScrollViewer);
        _mainGrid.Children.Add(rightScrollViewer);
        _mainGrid.Children.Add(_bottomTab);
        _mainGrid.Children.Add(verticalSplitter1);
        _mainGrid.Children.Add(verticalSplitter2);
        _mainGrid.Children.Add(horizontalSplitter);

        // Добавляем Grid как единственный дочерний элемент панели
        LogicalChildren.Add(_mainGrid);
        VisualChildren.Add(_mainGrid);
    }

    public static readonly AttachedProperty<WorkspaceDock> DockProperty =
        AvaloniaProperty.RegisterAttached<WorkspacePanel, Control, WorkspaceDock>("Dock");

    public static void SetDock(Control obj, WorkspaceDock value) =>
        obj.SetValue(DockProperty, value);

    public static WorkspaceDock GetDock(Control obj) => obj.GetValue(DockProperty);

    protected override void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                RefreshChildren();
                break;

            case NotifyCollectionChangedAction.Move:
                RefreshChildren();
                break;

            case NotifyCollectionChangedAction.Remove:
                RefreshChildren();
                break;

            case NotifyCollectionChangedAction.Replace:
                RefreshChildren();
                break;

            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        RefreshChildren();
        return base.ArrangeOverride(finalSize);
    }

    private void RefreshChildren()
    {
        foreach (var control in Children)
        {
            var dock = GetDock(control);
            switch (dock)
            {
                case WorkspaceDock.Left:
                    if (_leftPanel.Children.Contains(control))
                    {
                        continue;
                    }

                    _rightPanel.Children.Remove(control);
                    _bottomPanel.Remove(control);
                    _leftPanel.Children.Add(control);
                    break;
                case WorkspaceDock.Right:
                    if (_rightPanel.Children.Contains(control))
                    {
                        continue;
                    }

                    _leftPanel.Children.Remove(control);
                    _bottomPanel.Remove(control);
                    _rightPanel.Children.Add(control);
                    break;
                case WorkspaceDock.Bottom:
                    if (_bottomPanel.Contains(control))
                    {
                        continue;
                    }

                    _leftPanel.Children.Remove(control);
                    _rightPanel.Children.Remove(control);
                    _bottomPanel.Add(control);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
