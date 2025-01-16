using System.Composition;
using Avalonia.Controls;

namespace Asv.Avalonia
{
    /// <summary>
    /// This attribute is used to find a matching View for the ViewModel in CompositionViewLocator.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportViewForAttribute : ExportAttribute
    {
        public ExportViewForAttribute(Type viewModelType)
            : base(viewModelType.FullName, typeof(Control))
        {
            if (viewModelType.IsSubclassOf(typeof(Control)))
            {
                throw new ArgumentException(
                    $"{viewModelType} cannot be a View type. It must be ViewModel type",
                    nameof(viewModelType)
                );
            }
        }
    }
}
