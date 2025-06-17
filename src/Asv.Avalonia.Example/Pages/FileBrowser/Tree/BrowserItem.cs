using Asv.Mavlink;
using Avalonia.Media;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia.Example;

public class BrowserItem : HeadlinedViewModel, IBrowserItem
{
    public BrowserItem(
        NavigationId id,
        string? parentPath,
        string path,
        ILoggerFactory loggerFactory
    )
        : base(id, loggerFactory)
    {
        ParentPath = parentPath;
        Path = path;
        Order = 0;
    }

    public string Path
    {
        get;
        set => SetField(ref field, value);
    } = string.Empty;

    public string? ParentPath
    {
        get;
        set => SetField(ref field, value);
    }

    public FileSize? Size
    {
        get;
        set => SetField(ref field, value);
    }

    public bool HasChildren
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsExpanded
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsSelected
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsInEditMode
    {
        get;
        set => SetField(ref field, value);
    }

    public string EditedName
    {
        get;
        set => SetField(ref field, value);
    } = string.Empty;

    public string? Crc32Hex
    {
        get;
        set => SetField(ref field, value);
    }

    public SolidColorBrush Crc32Color
    {
        get;
        set => SetField(ref field, value);
    } = null!;

    public FtpEntryType FtpEntryType
    {
        get;
        set => SetField(ref field, value);
    }
}
