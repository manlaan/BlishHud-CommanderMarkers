using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.RtApi;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using System;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class CornerIconSettingsView : View
{
    protected SettingService _settings;
    private Label? _rtApiStatusLabel;

    protected override void Build(Container buildPanel)
    {
        _settings = Service.Settings;

        base.Build(buildPanel);

        if (RtApiIntegrationHelper.IsEnabled)
        {
            Service.RtApiConnection?.EnsureActive();
        }

        if (Service.RtApiConnection != null)
        {
            Service.RtApiConnection.ConnectionStateChanged -= OnRtApiConnectionStateChanged;
            Service.RtApiConnection.ConnectionStateChanged += OnRtApiConnectionStateChanged;
        }

        var panel = new FlowPanel()
            .BeginFlow(buildPanel)
            .AddString("Top-left menu bar icon settings")
            .AddSetting(_settings.CornerIconEnabled)
            .AddSpace()
            .AddSettingEnum(_settings.CornerIconLeftClickAction)
            .AddSpace()
            .AddSettingEnum(_settings.CornerIconTexture)
            .AddSpace()
            .AddSetting(_settings.CornerIconPriority)
            .AddSpace(40)
            .AddString("External Data Integrations")
            .AddSpace(20)
            .AddSetting(_settings.RtApiIntegrationEnabled)
            .AddFlowControl(new Label()
            {
                Text = RtApiStatusText.ForState(Service.RtApiConnection?.State ?? RtApiConnectionState.NotDetected),
                AutoSizeWidth = true,
            }, out var rtApiStatusLabel)
            .AddSpace(40)
            .AddFlowControl(new StandardButton
            {
                Text = "Update Notes",
                BasicTooltipText = "Open the module update notes in your default web browser",
            }, out var patchNotesButton)
            ;

        _rtApiStatusLabel = rtApiStatusLabel as Label;

        _settings.RtApiIntegrationEnabled.SettingChanged += OnRtApiIntegrationSettingChanged;

        patchNotesButton.Click += (s, e) =>
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://pkgs.blishhud.com/Manlaan.CommanderMarkers.html",
                UseShellExecute = true
            });
        };
        new Label()
        {
            Parent = buildPanel,
            Location = new Point(20, buildPanel.Height - 50),
            Text = "Special Thank You to the testers: QuitarHero, Kami, and Naru\nand to Metallis for the module icon",
            AutoSizeWidth = true,
            AutoSizeHeight = true,
        };
    }

    private void OnRtApiConnectionStateChanged(object? sender, RtApiConnectionState state)
    {
        if (_rtApiStatusLabel != null)
        {
            _rtApiStatusLabel.Text = RtApiStatusText.ForState(state);
        }
    }

    private void OnRtApiIntegrationSettingChanged(object? sender, ValueChangedEventArgs<bool> e)
    {
        if (e.NewValue)
        {
            Service.RtApiConnection?.EnsureActive();
        }

        if (_rtApiStatusLabel != null)
        {
            _rtApiStatusLabel.Text = RtApiStatusText.ForState(Service.RtApiConnection?.State ?? RtApiConnectionState.NotDetected);
        }
    }
}
