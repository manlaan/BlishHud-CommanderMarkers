using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Manlaan.CommanderMarkers.Settings.Views.Tabs;
using Manlaan.CommanderMarkers.Settings.Views.Generic;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Settings.Controls;

public class SettingsPanel : TabbedWindow2
{
    private static Texture2D? Background => Service.Textures?.SettingWindowBackground;

    public ModuleSettingsTab modSettingsTab = new ModuleSettingsTab();
    //Where on the background texture should the panel render
    private static Rectangle SettingPanelRegion => new()
    {
        Location = new Point(38, 25),
        Size = new Point(1100, 705),
    };
    
    private static Rectangle SettingPanelContentRegion => new()
    {
        Location = SettingPanelRegion.Location + new Point(52, 0),
        Size = SettingPanelRegion.Size - SettingPanelRegion.Location,
    };
    private static Point SettingPanelWindowSize => new(800, 600);

    public SettingsPanel() : base(Background, SettingPanelRegion, SettingPanelContentRegion, SettingPanelWindowSize)
    {
        Id = $"{nameof(Module)}_96aaaa83-4163-4d97-b894-282406b29a49";
        Emblem = Service.Textures?._blishHeart;
        Parent = GameService.Graphics.SpriteScreen;
        Title = "Commander Markers";
        Subtitle = "configuration";
        SavesPosition = true;

        BuildTabs();
        
#if DEBUG
        Task.Delay(500).ContinueWith(_ => Show());
#endif
    }
    
    private void BuildTabs()
    {
        Tabs.Add(new Tab(
            Service.Textures?._imgHeart,
            () => new CustomSettingMenuView(modSettingsTab),
            "Settings"
        ));
       
    }

    public void ShowLibrary()
    {
        Show();
        modSettingsTab.ActivateLibraryTab();
    }
}
