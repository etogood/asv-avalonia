using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace Asv.Avalonia;

/*public class SelectionTextBlock : TextBlock
{
    static SelectionTextBlock()
    {
        // Когда текст в TextBlock меняется, пересобираем Inlines
        TextProperty.Changed.AddClassHandler<SelectionTextBlock>((tb, e) => tb.OnTextOrSelectionChanged());

        // Когда у нас меняется Selection, тоже пересобираем Inlines
        SelectionProperty.Changed.AddClassHandler<SelectionTextBlock>((tb, e) => tb.OnTextOrSelectionChanged());
    }
    
    public SelectionTextBlock()
    {
        OnTextOrSelectionChanged();
    }
    
    private void OnTextOrSelectionChanged()
    {
        if (Inlines == null)
        {
            Inlines = new InlineCollection();
        }
        
        Inlines.Clear();

        var fullText = Text ?? string.Empty;
        var sel = Selection;

        // Если нет текста или длина выделения ≤ 0, просто выводим весь текст без подсветки
        if (string.IsNullOrEmpty(fullText) || sel.Length <= 0)
        {
            Inlines.Add(new Run(fullText));
            return;
        }

        // Вычисляем границы выделения, чтобы не выйти за пределы строки
        int start = sel.Start;
        int length = sel.Length;
        if (start < 0)
        {
            start = 0;
        }

        if (length < 0)
        {
            length = 0;
        }

        if (start > fullText.Length)
        {
            start = fullText.Length;
        }

        if (start + length > fullText.Length)
        {
            length = fullText.Length - start;
        }

        if (start > 0)
        {
            string prefix = fullText.Substring(0, start);
            Inlines.Add(new Run(prefix));
        }

        // 2) Сам выделенный фрагмент (с жёлтым фоном)
        if (length > 0)
        {
            string selectedPortion = fullText.Substring(start, length);
            var runSel = new Run(selectedPortion)
            {
                Background = Brushes.Yellow,
                Foreground = Brushes.Black,
            };
            Inlines.Add(runSel);
        }

        // 3) Остальная часть текста после выделения
        int after = start + length;
        if (after < fullText.Length)
        {
            string suffix = fullText.Substring(after);
            Inlines.Add(new Run(suffix));
        }
    }
    
    public static readonly StyledProperty<Selection> SelectionProperty =
        AvaloniaProperty.Register<SelectionTextBlock, Selection>(
            nameof(Selection), defaultValue: new Selection(0, 0)
        );
    
    public Selection Selection
    {
        get => GetValue(SelectionProperty);
        set => SetValue(SelectionProperty, value);
    }
}*/
