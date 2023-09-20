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

public class CornerIconSettingsView : View
{
    protected SettingService _settings;
    protected override void Build(Container buildPanel)
    {
        _settings = Service.Settings;

        base.Build(buildPanel);

        var panel = new FlowPanel()
            .BeginFlow(buildPanel)
            .AddString("Top-left menu bar icon settings")
            .AddSetting(_settings.CornerIconEnabled)
            .AddSpace()
            .AddSettingEnum(_settings.CornerIconLeftClickAction)
            .AddSpace(100)
            .AddFlowControl(new StandardButton
            {
                Text = "Update Notes",
                BasicTooltipText = "Open the module update notes in your default web browser",
            }, out var patchNotesButton)
            ;

        patchNotesButton.Click += (s, e) =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://pkgs.blishhud.com/Manlaan.CommanderMarkers.html",
                UseShellExecute = true
            });
        };
        new Label()
        {
            Parent = buildPanel,
            Location = new Point(20, buildPanel.Height - 30),
            Text = "Special Thank You to the testers: QuitarHero, Kami, and Naru",
            AutoSizeWidth = true,
        };
    }
}