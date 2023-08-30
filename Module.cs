using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Controls.Extern;
using Blish_HUD.Input;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Markers;
using Manlaan.CommanderMarkers.Presets;
using System.IO;
using MonoGame.Extended;
using Manlaan.CommanderMarkers.Utils;
using System.Threading;
using SharpDX.Direct2D1;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using System.Collections.Generic;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using Manlaan.CommanderMarkers.Presets.Service;
using Manlaan.CommanderMarkers.Presets.Model;

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


        }

        protected override void Update(GameTime gameTime)
        {
            IconsPanel.Update(gameTime);
            Service.MapWatch.Update(gameTime);

        }

        /// <inheritdoc />
        protected override void Unload()
        {

            Service.MapWatch?.Dispose();

            IconsPanel?.Dispose();
            Service.Settings?.Dispose();
            Service.Textures?.Dispose();

            Service.MapDataCache?.Dispose();

            Service.SettingsWindow?.Dispose();
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