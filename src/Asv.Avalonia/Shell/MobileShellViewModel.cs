using System.Composition;
using Asv.Cfg;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[Export(ShellId, typeof(IShell))]
public class MobileShellViewModel : ShellViewModel
{
    public const string ShellId = "shell.mobile";

    [ImportingConstructor]
    public MobileShellViewModel(
        IConfiguration cfg,
        IContainerHost containerHost,
        ILoggerFactory loggerFactory
    )
        : base(containerHost, loggerFactory, cfg, ShellId)
    {
        // do nothing
    }
}
