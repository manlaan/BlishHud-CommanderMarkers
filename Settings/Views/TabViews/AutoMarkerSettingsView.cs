using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings.UI.Views;
using Manlaan.CommanderMarkers.Settings.Controls;
using Manlaan.CommanderMarkers.Settings.Enums;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class AutoMarkerSettingsView : View
{
    protected SettingService _settings;
    protected override void Build(Container buildPanel)
    {
        _settings = Service.Settings;

        base.Build(buildPanel);


        var panel = new FlowPanel()
            .BeginFlow(buildPanel)
            .AddString("AutoMarker Feature")
            .AddString("Allows for rapidly placing saved marker sets")
            .AddSpace()
            .AddSetting(_settings.AutoMarker_FeatureEnabled)
            .AddSetting(_settings.AutoMarker_OnlyWhenCommander)
            .AddSpace()
            .AddSetting(_settings.AutoMarker_PlacementDelay)
            .AddFlowControl(new Label()
            {
                Text = $"  Delay Time: {_settings.AutoMarker_PlacementDelay.Value} ms",

                AutoSizeWidth = true,
            }, out var delayLabel);

        new Label()
        {
            Parent = buildPanel,
            Text = "Press and hold Ctrl and Shift to activate the button",
            AutoSizeWidth = true,
            Location = new Point(0, buildPanel.Height - 65)
        };
        var ResetButton = new NuclearOptionButton()
        {
            Parent = buildPanel,
            Text = $"Reset Library To Default",
            BasicTooltipText = "Warning: This will delete ALL marker sets in your Library\nand restore the default markers.\n\nPress and hold Ctrl and Shift to activate the button",
            Width = 200,
            Location = new Point(0, buildPanel.Height - 35)
        };
        ResetButton.Click += (s, e) =>
        {
            Service.MarkersListing.ResetToDefault();
            ScreenNotification.ShowNotification("AutoMarker Library has been reset", ScreenNotification.NotificationType.Gray, null, 4);

        };

        new Image()
        {
            Parent = buildPanel,
            Texture = Service.Textures!._blishHeart,
            Size = new Point(96, 96),
            Location = new Point(buildPanel.Width - 96, buildPanel.Height - 96),
        };

        _settings.AutoMarker_PlacementDelay.SettingChanged += (s, e) =>
        {
            (delayLabel as Label).Text = $"  Delay Time: {_settings.AutoMarker_PlacementDelay.Value} ms";
        };

    }

}