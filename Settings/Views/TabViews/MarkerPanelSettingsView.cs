using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings.UI.Views;
using Manlaan.CommanderMarkers.Markers;
using Manlaan.CommanderMarkers.Settings.Enums;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class MarkerPanelSettingsView : View
{
    protected SettingService _settings;
    protected override void Build(Container buildPanel)
    {
        _settings = Service.Settings;

        base.Build(buildPanel);

        var panel = new FlowPanel()
            .BeginFlow(buildPanel)
            .AddSetting(_settings._settingShowMarkersPanel)
            .AddSetting(_settings._settingOnlyWhenCommander)
            .AddSpace()
            .AddSetting(_settings._settingDrag)
            .AddSpace()
            .AddSetting(_settings._settingImgWidth)
            .AddSetting(_settings._settingOpacity)
            .AddSettingEnum(_settings._settingOrientation)
            .AddSpace()
            .AddString("Preview")
            .AddSpace()
            .AddControl(new MarkersPanel(Service.Settings, Service.Textures!, false));

    }
}