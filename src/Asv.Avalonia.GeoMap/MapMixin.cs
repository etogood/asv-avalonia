using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asv.Avalonia.GeoMap;

public static class MapMixin
{
    public static IHostApplicationBuilder UseAsvMap(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ITileLoader, TileLoader>();
        return builder;
    }
}