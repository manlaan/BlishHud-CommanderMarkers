using System.ComponentModel;

namespace Manlaan.CommanderMarkers.Settings.Enums;

public enum CornerIconActions
{
    [Description("Show Quick Access Menu")]
    SHOW_ICON_MENU,

    [Description("Show The Settings Window")]
    SHOW_SETTINGS,

    [Description("Open The Marker library")]
    LIBRARY,

    [Description("Lieutenant's Mode")]
    LIEUTENANT,

    [Description("Toggle Markers Panel Visibility")]
    CLICKMARKER_TOGGLE,


}