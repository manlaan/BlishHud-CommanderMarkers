using System.Reflection;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Blish_HUD.Settings.UI.Views;

namespace Manlaan.CommanderMarkers.Settings.Views.Generic;

public class CustomSettingMenuView : SettingsMenuView
{
    protected static int NEW_MENU_WIDTH = 200;
    public CustomSettingMenuView(ISettingsMenuRegistrar settingsMenuRegistrar) : base(settingsMenuRegistrar)
    {
    }

    protected override void Build(Container buildPanel)
    {
        base.Build(buildPanel);

        if (GetMenuPanel() is { } menuPanel)
        {
            //(menuPanel.Parent as Panel).Title = "";
            menuPanel.Parent.Size = new(NEW_MENU_WIDTH, buildPanel.Height);

            menuPanel.Size = menuPanel.Parent.ContentRegion.Size;

        }
        if (GetViewContainer() is { } viewContainer)
        {
            viewContainer.Size = new(buildPanel.ContentRegion.Width - NEW_MENU_WIDTH - (int)buildPanel.Padding.Left, buildPanel.ContentRegion.Height);
            viewContainer.Location = new(NEW_MENU_WIDTH + 5, 10);
        }

    }

    private Menu? GetMenuPanel()
    {
        var fieldInfo = typeof(SettingsMenuView).GetField("_menuSettingsList", BindingFlags.NonPublic | BindingFlags.Instance);

        return fieldInfo?.GetValue(this) as Menu;
    }

    private ViewContainer? GetViewContainer()
    {
        var fieldInfo = typeof(SettingsMenuView).GetField("_settingViewContainer", BindingFlags.NonPublic | BindingFlags.Instance);

        return fieldInfo?.GetValue(this) as ViewContainer;
    }

}