using System.Composition;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.Logging;

namespace Asv.Avalonia;

[Export(ShellId, typeof(IShell))]
public class MobileShellViewModel : ShellViewModel
{
    public const string ShellId = "shell.mobile";

    [ImportingConstructor]
    public MobileShellViewModel(IContainerHost containerHost, ILoggerFactory loggerFactory)
        : base(containerHost, loggerFactory, ShellId)
    {
        // do nothing
    }
}
