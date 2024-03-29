﻿using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Settings;
using Blish_HUD.Input;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Markers;
using Manlaan.CommanderMarkers.Presets;
using System.IO;
using Manlaan.CommanderMarkers.Utils;
using System.Threading;
using System.Collections.Generic;
using Manlaan.CommanderMarkers.Presets.Services;
using Manlaan.CommanderMarkers.CornerIcon;
using Manlaan.CommanderMarkers.Settings.Enums;
using Manlaan.CommanderMarkers.Library.Controls;

namespace Manlaan.CommanderMarkers
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {
        public static string DIRECTORY_PATH = "commanderMarkers"; //Defined folder in manifest.json

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        public static SettingService Settings { get; set; } = null!;
        public static TextureService? Textures { get; set; } = null;
        public static MarkersPanel IconsPanel { get; set; } = null!;


        public static string[] _orientation = new string[] { "Horizontal", "Vertical" };

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
            Service.ModuleInstance = this;
            Service.ContentsManager = moduleParameters.ContentsManager;
            Service.Gw2ApiManager = moduleParameters.Gw2ApiManager;
            Service.DirectoriesManager = moduleParameters.DirectoriesManager;
        }
        protected override void DefineSettings(SettingCollection settings) => Settings = Service.Settings = new SettingService(settings);

        public override IView GetSettingsView() => new Settings.Views.ModuleSettingsView(Settings);

        protected override async Task LoadAsync()
        {
            Service.Textures = new TextureService(Service.ContentsManager);
            IconsPanel = new MarkersPanel(Settings, Service.Textures);

            Service.MapDataCache = new MapData(GetCacheFile().FullName);

            Service.MarkersListing = MarkerListing.Load();
            Service.MapWatch = new MapWatchService(Service.MapDataCache, Settings);

            Service.SettingsWindow = new();


            //var refreshApiContextMenu = new ContextMenuStripItem(Strings.Settings_RefreshNow);
            //refreshApiContextMenu.Click += (s, e) => Service.ApiPollingService?.Invoke();

            var LtMode = new ContextMenuStripItem("Lieutenant's Mode")
            {
                BasicTooltipText = "Temporarily override the 'Only While Commander' settings",
                CanCheck = true,
                Checked = false
            };
            LtMode.CheckedChanged += (s, e) => Service.LtMode.Value = e.Checked;
            Service.CornerIcon = new CornerIconService(
                Service.Settings.CornerIconEnabled,
                "Commander Markers",
                new List<ContextMenuStripItem>()
                {
                    new CornerIconToggleMenuItem(Service.SettingsWindow, "Open Settings"),
                    new LibrayCornerIconMenuItem(Service.Settings.AutoMarker_FeatureEnabled, "Open Library"),
                    new ContextMenuStripItemSeparator(),
                    LtMode,

                }
            );

            Service.CornerIcon.IconLeftClicked += CornerIcon_IconLeftClicked;


        }
        private void CornerIcon_IconLeftClicked(object sender, bool e)
        {
            switch (Service.Settings.CornerIconLeftClickAction.Value)
            {
                case CornerIconActions.SHOW_ICON_MENU:
                    Service.CornerIcon?.OpenContextMenu();
                    break;
                case CornerIconActions.SHOW_SETTINGS:
                    Service.SettingsWindow.Show();
                    break;
                case CornerIconActions.LIEUTENANT:
                    LieutentantMode();
                    break;

                case CornerIconActions.LIBRARY:
                    Service.SettingsWindow.ShowLibrary();
                    break;
                case CornerIconActions.CLICKMARKER_TOGGLE:
                    Service.Settings._settingShowMarkersPanel.Value = !Service.Settings._settingShowMarkersPanel.Value;
                    break;
            }
        }
        private void LieutentantMode()
        {
            Service.LtMode.Value = !Service.LtMode.Value;
        }

        protected override void Update(GameTime gameTime)
        {
            IconsPanel?.Update(gameTime);
            Service.MapWatch?.Update(gameTime);

        }

        /// <inheritdoc />
        protected override void Unload()
        {
            if(Service.CornerIcon != null)
                Service.CornerIcon.IconLeftClicked += CornerIcon_IconLeftClicked;


            Service.CornerIcon?.Dispose();
            Service.SettingsWindow?.Dispose();

            Service.MapWatch?.Dispose();
            Service.MapDataCache?.Dispose();

            IconsPanel?.Dispose();
            Service.Settings?.Dispose();
            Service.Textures?.Dispose();

            

        }

        private FileInfo GetCacheFile()
        {
            var pluginConfigDirectory = Service.DirectoriesManager.GetFullDirectoryPath(Module.DIRECTORY_PATH);

            return new FileInfo($@"{pluginConfigDirectory}\{MapData.MAPDATA_CACHE_FILENAME}");
        }

    }


    public class MarkerSequence{
        private int _mapId;
        List<Vector3> _markers = new List<Vector3>() { };

        public MarkerSequence(int mapId, List<Vector3> markers)
        {
            _mapId = mapId;
            _markers = markers;
        }

        public void PlaceMarkers(MapData mapData)
        {
            var scale = GameService.Graphics.UIScaleMultiplier;
            var ingameScale = GameService.Gw2Mumble.UI.UISize;
            var key = Keys.D1;
            if (GameService.Gw2Mumble.CurrentMap.Id != _mapId) return;
            InputHelper.DoHotKey(new KeyBinding(ModifierKeys.Alt, Keys.D9));
            Thread.Sleep(20);
            for (var i=0; i< _markers.Count ; i++)
            {
                var d = mapData.WorldToScreenMap(_markers[i]) * scale;
                Mouse.SetPosition((int)d.X , (int)d.Y);
                Thread.Sleep(10);
                InputHelper.DoHotKey(new KeyBinding(ModifierKeys.Alt, key+i));
                Thread.Sleep(60);
            }

        }
    }

}