namespace Asv.Avalonia;

public class AppPath:IAppPath
{
    public required string UserDataFolder { get; set; }
    public required string AppFolder { get; set; }
}