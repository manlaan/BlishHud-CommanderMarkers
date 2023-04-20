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

namespace Manlaan.CommanderMarkers
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        public static SettingService Settings { get; set; } = null!;
        public static TextureService? Textures { get; set; } = null;
        public static MarkersPanel IconsPanel { get; set; } = null!;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        public static string[] _orientation = new string[] { "Horizontal", "Vertical" };

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
        }
        protected override void DefineSettings(SettingCollection settings) => Settings = new SettingService(settings);

        public override IView GetSettingsView() => new Settings.Views.ModuleSettingsView(Settings);

        protected override async Task LoadAsync()
        {
            Textures = new TextureService(ContentsManager);
            IconsPanel = new MarkersPanel(Settings, Textures);
        }

        protected override void Update(GameTime gameTime)
        {
            IconsPanel.Update(gameTime);
            //if (GameService.GameIntegration.Gw2Instance.IsInGame && !GameService.Gw2Mumble.UI.IsMapOpen && GameService.Gw2Mumble.PlayerCharacter.IsCommander)
            //{
            //_cmdPanel.Show();
           /* }
            else
            {
                _cmdPanel.Hide();
            }*/
            /*if (_dragging)
            {
                var nOffset = InputService.Input.Mouse.Position - _dragStart;
                _cmdPanel.Location += nOffset;

                _dragStart = InputService.Input.Mouse.Position;
                _cmdPanel.Show();
            }*/
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            IconsPanel?.Dispose();
            Settings?.Dispose();
            Textures?.Dispose();
            
        }
    }

}
