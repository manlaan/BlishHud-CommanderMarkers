using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.CornerIcon;
using Manlaan.CommanderMarkers.Library.Services;
using Manlaan.CommanderMarkers.Presets;
using Manlaan.CommanderMarkers.Presets.Services;
using Manlaan.CommanderMarkers.Services;
using Manlaan.CommanderMarkers.Settings.Controls;
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
    public static MapData MapDataCache { get; set; } = null!;

    public static SettingsPanel SettingsWindow { get; set; } = null!;

    public static CornerIconService? CornerIcon { get; set; } = null;

    public static CommanderMarkersManifestService ManifestService { get; set; } = null!;
    public static CommunityCatalogService CommunityCatalog { get; set; } = null!;
    public static PreviewImageCache PreviewImageCache { get; set; } = null!;
    public static SubtokenService SubtokenService { get; set; } = null!;

    public static RtApiConnection? RtApiConnection { get; set; }
    public static RtApiEventListener? RtApiEvents { get; set; }

    public static string? AccountDisplayName { get; set; }

    public static SettingEntry<bool> LtMode { get; set; } = new SettingEntry<bool>()
    {
        Value = false
    };


}