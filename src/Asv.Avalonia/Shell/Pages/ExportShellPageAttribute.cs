using System.Composition;

namespace Asv.Avalonia;

/// <summary>
/// This attribute is used to find a matching View for the ViewModel in CompositionViewLocator.
/// </summary>
[MetadataAttribute]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExportShellPageAttribute : ExportAttribute
{
    public ExportShellPageAttribute(string pageId)
        : base(pageId, typeof(IShellPage))
    {
    }
}