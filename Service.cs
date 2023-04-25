using Blish_HUD.Modules.Managers;
using Manlaan.CommanderMarkers.Presets.Service;
using Manlaan.CommanderMarkers.Settings.Services;

namespace Manlaan.CommanderMarkers;

public static class Service 
{
    public static Module ModuleInstance { get; set; } = null!;
    public static SettingService Settings { get; set; } = null!;
    public static ContentsManager ContentsManager { get; set; } = null!;
    public static Gw2ApiManager Gw2ApiManager { get; set; } = null!;
    public static DirectoriesManager DirectoriesManager { get; set; } = null!;
    public static TextureService? Textures { get; set; }

    public static MarkerListing MarkersListing { get; set; } = null!;
    public static MapWatchService MapWatch { get; set; } = null!;

}