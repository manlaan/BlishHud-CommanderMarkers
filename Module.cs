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

namespace Manlaan.CommanderMarkers
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        public enum Orientation { Horizontal, Vertical }
        private SettingEntry<KeyBinding> _settingArrowGndBinding;
        private SettingEntry<KeyBinding> _settingCircleGndBinding;
        private SettingEntry<KeyBinding> _settingHeartGndBinding;
        private SettingEntry<KeyBinding> _settingSpiralGndBinding;
        private SettingEntry<KeyBinding> _settingSquareGndBinding;
        private SettingEntry<KeyBinding> _settingStarGndBinding;
        private SettingEntry<KeyBinding> _settingTriangleGndBinding;
        private SettingEntry<KeyBinding> _settingXGndBinding;
        private SettingEntry<KeyBinding> _settingClearGndBinding;
        private SettingEntry<KeyBinding> _settingArrowObjBinding;
        private SettingEntry<KeyBinding> _settingCircleObjBinding;
        private SettingEntry<KeyBinding> _settingHeartObjBinding;
        private SettingEntry<KeyBinding> _settingSpiralObjBinding;
        private SettingEntry<KeyBinding> _settingSquareObjBinding;
        private SettingEntry<KeyBinding> _settingStarObjBinding;
        private SettingEntry<KeyBinding> _settingTriangleObjBinding;
        private SettingEntry<KeyBinding> _settingXObjBinding;
        private SettingEntry<KeyBinding> _settingClearObjBinding;

        private SettingEntry<Orientation> _settingOrientation;
        private SettingEntry<string> _settingLocX;
        private SettingEntry<string> _settingLocY;
        private SettingEntry<int> _settingImgWidth;
        private SettingEntry<float> _settingOpacity;

        private Image _btnArrowGnd;
        private Image _btnCircleGnd;
        private Image _btnHeartGnd;
        private Image _btnSpiralGnd;
        private Image _btnSquareGnd;
        private Image _btnStarGnd;
        private Image _btnTriangleGnd;
        private Image _btnXGnd;
        private Image _btnClearGnd;

        private Image _btnArrowObj;
        private Image _btnCircleObj;
        private Image _btnHeartObj;
        private Image _btnSpiralObj;
        private Image _btnSquareObj;
        private Image _btnStarObj;
        private Image _btnTriangleObj;
        private Image _btnXObj;
        private Image _btnClearObj;

        private Texture2D _imgArrow;
        private Texture2D _imgCircle;
        private Texture2D _imgHeart;
        private Texture2D _imgSpiral;
        private Texture2D _imgSquare;
        private Texture2D _imgStar;
        private Texture2D _imgTriangle;
        private Texture2D _imgX;
        private Texture2D _imgClear;

        private KeyBinding _tmpBinding;
        private Image _tmpButton;


        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

        protected override void Initialize()
        {
            _imgArrow = ContentsManager.GetTexture(@"arrow.png");
            _imgCircle = ContentsManager.GetTexture(@"circle.png");
            _imgHeart = ContentsManager.GetTexture(@"heart.png");
            _imgSpiral = ContentsManager.GetTexture(@"spiral.png");
            _imgSquare = ContentsManager.GetTexture(@"square.png");
            _imgStar = ContentsManager.GetTexture(@"star.png");
            _imgTriangle = ContentsManager.GetTexture(@"triangle.png");
            _imgX = ContentsManager.GetTexture(@"x.png");
            _imgClear = ContentsManager.GetTexture(@"clear.png");
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingArrowGndBinding = settings.DefineSetting("CmdMrkArrowGndBinding", new KeyBinding(ModifierKeys.Alt,Keys.D1), "Arrow Ground Binding", "");
            _settingCircleGndBinding = settings.DefineSetting("CmdMrkCircleGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D2), "Circle Ground Binding", "");
            _settingHeartGndBinding = settings.DefineSetting("CmdMrkHeartGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D3), "Heart Ground Binding", "");
            _settingSquareGndBinding = settings.DefineSetting("CmdMrkSquareGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D4), "Square Ground Binding", "");
            _settingStarGndBinding = settings.DefineSetting("CmdMrkStarGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D5), "Star Ground Binding", "");
            _settingSpiralGndBinding = settings.DefineSetting("CmdMrkSpiralGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D6), "Spiral Ground Binding", "");
            _settingTriangleGndBinding = settings.DefineSetting("CmdMrkTriangleGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D7), "Triangle Ground Binding", "");
            _settingXGndBinding = settings.DefineSetting("CmdMrkXGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D8), "X Ground Binding", "");
            _settingClearGndBinding = settings.DefineSetting("CmdMrkClearGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D9), "Clear Ground Binding", "");

            _settingArrowObjBinding = settings.DefineSetting("CmdMrkArrowObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D1), "Arrow Object Binding", "");
            _settingCircleObjBinding = settings.DefineSetting("CmdMrkCircleObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D2), "Circle Object Binding", "");
            _settingHeartObjBinding = settings.DefineSetting("CmdMrkHeartObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D3), "Heart Object Binding", "");
            _settingSquareObjBinding = settings.DefineSetting("CmdMrkSquareObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D4), "Square Object Binding", "");
            _settingStarObjBinding = settings.DefineSetting("CmdMrkStarObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D5), "Star Object Binding", "");
            _settingSpiralObjBinding = settings.DefineSetting("CmdMrkSpiralObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D6), "Spiral Object Binding", "");
            _settingTriangleObjBinding = settings.DefineSetting("CmdMrkTriangleObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D7), "Triangle Object Binding", "");
            _settingXObjBinding = settings.DefineSetting("CmdMrkXObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D8), "X Object Binding", "");
            _settingClearObjBinding = settings.DefineSetting("CmdMrkClearObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D9), "Clear Object Binding", "");

            _settingOrientation = settings.DefineSetting("CmdMrkOrientation", Orientation.Horizontal, "Orientation", "");
            _settingLocX = settings.DefineSetting("CmdMrknLocX", "60", "X", "");
            _settingLocY = settings.DefineSetting("CmdMrkLocY", "40", "Y", "");
            _settingImgWidth = settings.DefineSetting("CmdMrkImgWidth", 30, "Width", "");
            _settingOpacity = settings.DefineSetting("CmdMrkOpacity", 1.0f, "Opacity", "");

            _settingImgWidth.SetRange(0, 200);
            _settingOpacity.SetRange(0f, 1f);

            _settingArrowGndBinding.SettingChanged += UpdateSettings;
            _settingCircleGndBinding.SettingChanged += UpdateSettings;
            _settingHeartGndBinding.SettingChanged += UpdateSettings;
            _settingSpiralGndBinding.SettingChanged += UpdateSettings;
            _settingSquareGndBinding.SettingChanged += UpdateSettings;
            _settingStarGndBinding.SettingChanged += UpdateSettings;
            _settingTriangleGndBinding.SettingChanged += UpdateSettings;
            _settingXGndBinding.SettingChanged += UpdateSettings;
            _settingClearGndBinding.SettingChanged += UpdateSettings;

            _settingArrowObjBinding.SettingChanged += UpdateSettings;
            _settingCircleObjBinding.SettingChanged += UpdateSettings;
            _settingHeartObjBinding.SettingChanged += UpdateSettings;
            _settingSpiralObjBinding.SettingChanged += UpdateSettings;
            _settingSquareObjBinding.SettingChanged += UpdateSettings;
            _settingStarObjBinding.SettingChanged += UpdateSettings;
            _settingTriangleObjBinding.SettingChanged += UpdateSettings;
            _settingXObjBinding.SettingChanged += UpdateSettings;
            _settingClearObjBinding.SettingChanged += UpdateSettings;

            _settingOrientation.SettingChanged += UpdateSettings;
            _settingLocX.SettingChanged += UpdateSettings;
            _settingLocY.SettingChanged += UpdateSettings;
            _settingImgWidth.SettingChanged += UpdateSettings;
            _settingOpacity.SettingChanged += UpdateSettings;
        }

        protected override async Task LoadAsync()
        {

        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            DrawIcons();
            GameService.Input.Mouse.LeftMouseButtonPressed += OnMouseClick;
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime)
        {

        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _btnArrowGnd?.Dispose();
            _btnCircleGnd?.Dispose();
            _btnHeartGnd?.Dispose();
            _btnSpiralGnd?.Dispose();
            _btnSquareGnd?.Dispose();
            _btnStarGnd?.Dispose();
            _btnTriangleGnd?.Dispose();
            _btnXGnd?.Dispose();
            _btnClearGnd?.Dispose();

            _btnArrowObj?.Dispose();
            _btnCircleObj?.Dispose();
            _btnHeartObj?.Dispose();
            _btnSpiralObj?.Dispose();
            _btnSquareObj?.Dispose();
            _btnStarObj?.Dispose();
            _btnTriangleObj?.Dispose();
            _btnXObj?.Dispose();
            _btnClearObj?.Dispose();

            _settingArrowGndBinding.SettingChanged -= UpdateSettings;
            _settingCircleGndBinding.SettingChanged -= UpdateSettings;
            _settingHeartGndBinding.SettingChanged -= UpdateSettings;
            _settingSpiralGndBinding.SettingChanged -= UpdateSettings;
            _settingSquareGndBinding.SettingChanged -= UpdateSettings;
            _settingStarGndBinding.SettingChanged -= UpdateSettings;
            _settingTriangleGndBinding.SettingChanged -= UpdateSettings;
            _settingXGndBinding.SettingChanged -= UpdateSettings;
            _settingClearGndBinding.SettingChanged -= UpdateSettings;

            _settingArrowObjBinding.SettingChanged -= UpdateSettings;
            _settingCircleObjBinding.SettingChanged -= UpdateSettings;
            _settingHeartObjBinding.SettingChanged -= UpdateSettings;
            _settingSpiralObjBinding.SettingChanged -= UpdateSettings;
            _settingSquareObjBinding.SettingChanged -= UpdateSettings;
            _settingStarObjBinding.SettingChanged -= UpdateSettings;
            _settingTriangleObjBinding.SettingChanged -= UpdateSettings;
            _settingXObjBinding.SettingChanged -= UpdateSettings;
            _settingClearObjBinding.SettingChanged -= UpdateSettings;

            _settingOrientation.SettingChanged -= UpdateSettings;
            _settingLocX.SettingChanged -= UpdateSettings;
            _settingLocY.SettingChanged -= UpdateSettings;
            _settingImgWidth.SettingChanged -= UpdateSettings;
            _settingOpacity.SettingChanged -= UpdateSettings;
           
        }


        private void UpdateSettings(object sender = null, ValueChangedEventArgs<KeyBinding> e = null)
        {
            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<Orientation> e = null)
        {
            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<float> e = null)
        {
            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<string> e = null)
        {
            if (int.Parse(_settingLocX.Value) < 0)
                _settingLocX.Value = "0";
            if (int.Parse(_settingLocY.Value) < 0)
                _settingLocY.Value = "0";

            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<int> e = null)
        {
            DrawIcons();
        }


        protected void DrawIcons()
        {
            _btnArrowGnd?.Dispose();
            _btnCircleGnd?.Dispose();
            _btnHeartGnd?.Dispose();
            _btnSpiralGnd?.Dispose();
            _btnSquareGnd?.Dispose();
            _btnStarGnd?.Dispose();
            _btnTriangleGnd?.Dispose();
            _btnXGnd?.Dispose();
            _btnClearGnd?.Dispose();

            _btnArrowObj?.Dispose();
            _btnCircleObj?.Dispose();
            _btnHeartObj?.Dispose();
            _btnSpiralObj?.Dispose();
            _btnSquareObj?.Dispose();
            _btnStarObj?.Dispose();
            _btnTriangleObj?.Dispose();
            _btnXObj?.Dispose();
            _btnClearObj?.Dispose();

            int curX = int.Parse(_settingLocX.Value);
            int curY = int.Parse(_settingLocY.Value);

            _btnArrowGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgArrow,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Arrow Ground"
            };
            _btnArrowGnd.LeftMouseButtonPressed += delegate { AddGround(_btnArrowGnd, _settingArrowGndBinding.Value); };
            _btnArrowGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingArrowGndBinding.Value); };

            _btnArrowObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgArrow,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Arrow Object"
            };
            _btnArrowObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingArrowObjBinding.Value); };
            _btnArrowObj.RightMouseButtonPressed += delegate { DoHotKey(_settingArrowObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            _btnCircleGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgCircle,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Circle Ground"
            };
            _btnCircleGnd.LeftMouseButtonPressed += delegate { AddGround(_btnCircleGnd, _settingCircleGndBinding.Value); };
            _btnCircleGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingCircleGndBinding.Value); };

            _btnCircleObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgCircle,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Circle Object"
            };
            _btnCircleObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingCircleObjBinding.Value); };
            _btnCircleObj.RightMouseButtonPressed += delegate { DoHotKey(_settingCircleObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            _btnHeartGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgHeart,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Heart Ground"
            };
            _btnHeartGnd.LeftMouseButtonPressed += delegate { AddGround(_btnHeartGnd, _settingHeartGndBinding.Value); };
            _btnHeartGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingHeartGndBinding.Value); };

            _btnHeartObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgHeart,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Heart Object"
            };
            _btnHeartObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingHeartObjBinding.Value); };
            _btnHeartObj.RightMouseButtonPressed += delegate { DoHotKey(_settingHeartObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            _btnSquareGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgSquare,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Square Ground"
            };
            _btnSquareGnd.LeftMouseButtonPressed += delegate { AddGround(_btnSquareGnd, _settingSquareGndBinding.Value); };
            _btnSquareGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingSquareGndBinding.Value); };

            _btnSquareObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgSquare,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Square Object"
            };
            _btnSquareObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingSquareObjBinding.Value); };
            _btnSquareObj.RightMouseButtonPressed += delegate { DoHotKey(_settingSquareObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            _btnStarGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgStar,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Star Ground"
            };
            _btnStarGnd.LeftMouseButtonPressed += delegate { AddGround(_btnStarGnd, _settingStarGndBinding.Value); };
            _btnStarGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingStarGndBinding.Value); };

            _btnStarObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgStar,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Star Object"
            };
            _btnStarObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingStarObjBinding.Value); };
            _btnStarObj.RightMouseButtonPressed += delegate { DoHotKey(_settingStarObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            _btnSpiralGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgSpiral,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Spiral Ground"
            };
            _btnSpiralGnd.LeftMouseButtonPressed += delegate { AddGround(_btnSpiralGnd, _settingSpiralGndBinding.Value); };
            _btnSpiralGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingSpiralGndBinding.Value); };

            _btnSpiralObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgSpiral,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Spiral Object"
            };
            _btnSpiralObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingSpiralObjBinding.Value); };
            _btnSpiralObj.RightMouseButtonPressed += delegate { DoHotKey(_settingSpiralObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            _btnTriangleGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgTriangle,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Triangle Ground"
            };
            _btnTriangleGnd.LeftMouseButtonPressed += delegate { AddGround(_btnTriangleGnd, _settingTriangleGndBinding.Value); };
            _btnTriangleGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingTriangleGndBinding.Value); };

            _btnTriangleObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgTriangle,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Triangle Object"
            };
            _btnTriangleObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingTriangleObjBinding.Value); };
            _btnTriangleObj.RightMouseButtonPressed += delegate { DoHotKey(_settingTriangleObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            _btnXGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgX,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "X Ground"
            };
            _btnXGnd.LeftMouseButtonPressed += delegate { AddGround(_btnXGnd, _settingXGndBinding.Value); };
            _btnXGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingXGndBinding.Value); };

            _btnXObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgX,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "X Object"
            };
            _btnXObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingXObjBinding.Value); };
            _btnXObj.RightMouseButtonPressed += delegate { DoHotKey(_settingXObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            _btnClearGnd = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgClear,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Clear Ground"
            };
            _btnClearGnd.LeftMouseButtonPressed += delegate { DoHotKey(_settingClearGndBinding.Value); };

            _btnClearObj = new Image
            {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Texture = _imgClear,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (_settingOrientation.Value == Orientation.Horizontal ? curX : curX + _settingImgWidth.Value),
                    (_settingOrientation.Value == Orientation.Horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Clear Object"
            };
            _btnClearObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingClearObjBinding.Value); };

            if (_settingOrientation.Value == Orientation.Horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;
        }

        private void OnMouseClick(object o, MouseEventArgs e)
        {
            if (_tmpBinding == null) return; 
            DoHotKey(_tmpBinding);
            _tmpButton.BackgroundColor = Color.Transparent;
            _tmpBinding = null;
            _tmpButton = null;
        }

        protected void AddGround(Image btn, KeyBinding key)
        {
            _tmpBinding = key;
            _tmpButton = btn;
            btn.BackgroundColor = Color.Blue;
        }
        protected void RemoveGround(KeyBinding key)
        {
            DoHotKey(key);
            System.Threading.Thread.Sleep(50);
            DoHotKey(key);
        }
        protected void DoHotKey(KeyBinding key)
        {
            if (key == null) return;
            if (key.ModifierKeys != ModifierKeys.None) {
                if (key.ModifierKeys.HasFlag(ModifierKeys.Alt))
                    Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.MENU, true);
                if (key.ModifierKeys.HasFlag(ModifierKeys.Ctrl))
                    Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.CONTROL, true);
                if (key.ModifierKeys.HasFlag(ModifierKeys.Shift))
                    Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.SHIFT, true);
            }
            Blish_HUD.Controls.Intern.Keyboard.Press(ToVirtualKey(key.PrimaryKey), true);
            System.Threading.Thread.Sleep(50);
            Blish_HUD.Controls.Intern.Keyboard.Release(ToVirtualKey(key.PrimaryKey), true);
            if (key.ModifierKeys != ModifierKeys.None)
            {
                if (key.ModifierKeys.HasFlag(ModifierKeys.Shift))
                    Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.SHIFT, true);
                if (key.ModifierKeys.HasFlag(ModifierKeys.Ctrl))
                    Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.CONTROL, true);
                if (key.ModifierKeys.HasFlag(ModifierKeys.Alt))
                    Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.MENU, true);
            }
        }
        private VirtualKeyShort ToVirtualKey(Keys key)
        {
            try
            {
                return (VirtualKeyShort)key;
            }
            catch
            {
                return new VirtualKeyShort();
            }
            /*
            foreach (VirtualKeyShort vkey in Enum.GetValues(typeof(VirtualKeyShort)))
            {
                if ((int)vkey == (int)key)
                    return vkey;
            }
            return new VirtualKeyShort();
            */
        }
    }

}
