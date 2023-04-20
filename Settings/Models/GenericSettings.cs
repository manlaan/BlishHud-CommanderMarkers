using Blish_HUD.Input;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Settings.Enums;
using Microsoft.Xna.Framework;
#pragma warning disable CS8618

namespace Manlaan.CommanderMarkers.Settings.Models;

public class GenericSettings
{
    public SettingEntry<bool> PositionLock { get; set; }
    public SettingEntry<Point> Location { get; set; }

    public SettingEntry<Layout> Layour { get; set; }
    public SettingEntry<bool> Tooltips { get; set; }
    public SettingEntry<bool> ToolbarIcon { get; set; }
    public SettingEntry<bool> Visible { get; set; }
    public SettingEntry<KeyBinding> ShowHideKeyBind { get; set; }

}