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

        public static string[] _orientation = new string[] { "Horizontal", "Vertical" };

        public static SettingEntry<KeyBinding> _settingArrowGndBinding;
        public static SettingEntry<KeyBinding> _settingCircleGndBinding;
        public static SettingEntry<KeyBinding> _settingHeartGndBinding;
        public static SettingEntry<KeyBinding> _settingSpiralGndBinding;
        public static SettingEntry<KeyBinding> _settingSquareGndBinding;
        public static SettingEntry<KeyBinding> _settingStarGndBinding;
        public static SettingEntry<KeyBinding> _settingTriangleGndBinding;
        public static SettingEntry<KeyBinding> _settingXGndBinding;
        public static SettingEntry<KeyBinding> _settingClearGndBinding;
        public static SettingEntry<KeyBinding> _settingArrowObjBinding;
        public static SettingEntry<KeyBinding> _settingCircleObjBinding;
        public static SettingEntry<KeyBinding> _settingHeartObjBinding;
        public static SettingEntry<KeyBinding> _settingSpiralObjBinding;
        public static SettingEntry<KeyBinding> _settingSquareObjBinding;
        public static SettingEntry<KeyBinding> _settingStarObjBinding;
        public static SettingEntry<KeyBinding> _settingTriangleObjBinding;
        public static SettingEntry<KeyBinding> _settingXObjBinding;
        public static SettingEntry<KeyBinding> _settingClearObjBinding;

        public static SettingEntry<string> _settingOrientation;
        private SettingEntry<Point> _settingLoc;
        public static SettingEntry<int> _settingImgWidth;
        public static SettingEntry<float> _settingOpacity;
        public static SettingEntry<bool> _settingDrag;

        private Texture2D _imgArrow;
        private Texture2D _imgCircle;
        private Texture2D _imgHeart;
        private Texture2D _imgSpiral;
        private Texture2D _imgSquare;
        private Texture2D _imgStar;
        private Texture2D _imgTriangle;
        private Texture2D _imgX;
        private Texture2D _imgClear;

        private Panel _cmdPanel;
        private bool _dragging;
        private Point _dragStart = Point.Zero;

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

            _settingOrientation = settings.DefineSetting("CmdMrkOrientation2", "Horizontal", "Orientation", "");
            _settingLoc = settings.DefineSetting("CmdMrkLoc", new Point(100,100), "Location", "");
            _settingImgWidth = settings.DefineSetting("CmdMrkImgWidth", 30, "Width", "");
            _settingOpacity = settings.DefineSetting("CmdMrkOpacity", 1.0f, "Opacity", "");
            _settingDrag = settings.DefineSetting("CmdMrkDrag", false, "Enable Dragging (White Box)", "");

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
            _settingLoc.SettingChanged += UpdateSettings;
            _settingImgWidth.SettingChanged += UpdateSettings;
            _settingOpacity.SettingChanged += UpdateSettings;
            _settingDrag.SettingChanged += UpdateSettings;
        }
        public override IView GetSettingsView() {
            return new CommanderMarkers.Views.SettingsView();
            //return new SettingsView( (this.ModuleParameters.SettingsManager.ModuleSettings);
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
            if (GameService.GameIntegration.Gw2Instance.IsInGame && !GameService.Gw2Mumble.UI.IsMapOpen && GameService.Gw2Mumble.PlayerCharacter.IsCommander) {
                _cmdPanel.Show();
            }
            else {
                _cmdPanel.Hide();
            }
            if (_dragging) {
                var nOffset = InputService.Input.Mouse.Position - _dragStart;
                _cmdPanel.Location += nOffset;

                _dragStart = InputService.Input.Mouse.Position;
            }
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            _cmdPanel?.Dispose();

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
            _settingLoc.SettingChanged -= UpdateSettings;
            _settingImgWidth.SettingChanged -= UpdateSettings;
            _settingOpacity.SettingChanged -= UpdateSettings;
            _settingDrag.SettingChanged -= UpdateSettings;
        }


        private void UpdateSettings(object sender = null, ValueChangedEventArgs<KeyBinding> e = null) {
            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<Point> e = null) {
            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<float> e = null)
        {
            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<string> e = null)
        {
            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<int> e = null) {
            DrawIcons();
        }
        private void UpdateSettings(object sender = null, ValueChangedEventArgs<bool> e = null) {
            DrawIcons();
        }


        protected void DrawIcons()
        {
            _cmdPanel?.Dispose();

            int curX = 0;
            int curY = 0;
            bool horizontal = _settingOrientation.Value.Equals("Horizontal");

            _cmdPanel = new Panel() {
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen,
                Location = _settingLoc.Value,
                Size = new Point(
                    horizontal ? _settingImgWidth.Value * 9 : _settingImgWidth.Value * 2,
                    horizontal ? _settingImgWidth.Value * 2 : _settingImgWidth.Value * 9),
            };

            Image _btnArrowGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgArrow,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(0, 0),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Arrow Ground"
            };
            _btnArrowGnd.LeftMouseButtonPressed += delegate { AddGround(_btnArrowGnd, _settingArrowGndBinding.Value); };
            _btnArrowGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingArrowGndBinding.Value); };

            Image _btnArrowObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgArrow,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Arrow Object"
            };
            _btnArrowObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingArrowObjBinding.Value); };
            _btnArrowObj.RightMouseButtonPressed += delegate { DoHotKey(_settingArrowObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            Image _btnCircleGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgCircle,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Circle Ground"
            };
            _btnCircleGnd.LeftMouseButtonPressed += delegate { AddGround(_btnCircleGnd, _settingCircleGndBinding.Value); };
            _btnCircleGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingCircleGndBinding.Value); };

            Image _btnCircleObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgCircle,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Circle Object"
            };
            _btnCircleObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingCircleObjBinding.Value); };
            _btnCircleObj.RightMouseButtonPressed += delegate { DoHotKey(_settingCircleObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            Image _btnHeartGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgHeart,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Heart Ground"
            };
            _btnHeartGnd.LeftMouseButtonPressed += delegate { AddGround(_btnHeartGnd, _settingHeartGndBinding.Value); };
            _btnHeartGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingHeartGndBinding.Value); };

            Image _btnHeartObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgHeart,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Heart Object"
            };
            _btnHeartObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingHeartObjBinding.Value); };
            _btnHeartObj.RightMouseButtonPressed += delegate { DoHotKey(_settingHeartObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            Image _btnSquareGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgSquare,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Square Ground"
            };
            _btnSquareGnd.LeftMouseButtonPressed += delegate { AddGround(_btnSquareGnd, _settingSquareGndBinding.Value); };
            _btnSquareGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingSquareGndBinding.Value); };

            Image _btnSquareObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgSquare,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Square Object"
            };
            _btnSquareObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingSquareObjBinding.Value); };
            _btnSquareObj.RightMouseButtonPressed += delegate { DoHotKey(_settingSquareObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            Image _btnStarGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgStar,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Star Ground"
            };
            _btnStarGnd.LeftMouseButtonPressed += delegate { AddGround(_btnStarGnd, _settingStarGndBinding.Value); };
            _btnStarGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingStarGndBinding.Value); };

            Image _btnStarObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgStar,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Star Object"
            };
            _btnStarObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingStarObjBinding.Value); };
            _btnStarObj.RightMouseButtonPressed += delegate { DoHotKey(_settingStarObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            Image _btnSpiralGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgSpiral,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Spiral Ground"
            };
            _btnSpiralGnd.LeftMouseButtonPressed += delegate { AddGround(_btnSpiralGnd, _settingSpiralGndBinding.Value); };
            _btnSpiralGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingSpiralGndBinding.Value); };

            Image _btnSpiralObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgSpiral,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Spiral Object"
            };
            _btnSpiralObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingSpiralObjBinding.Value); };
            _btnSpiralObj.RightMouseButtonPressed += delegate { DoHotKey(_settingSpiralObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            Image _btnTriangleGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgTriangle,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Triangle Ground"
            };
            _btnTriangleGnd.LeftMouseButtonPressed += delegate { AddGround(_btnTriangleGnd, _settingTriangleGndBinding.Value); };
            _btnTriangleGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingTriangleGndBinding.Value); };

            Image _btnTriangleObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgTriangle,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Triangle Object"
            };
            _btnTriangleObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingTriangleObjBinding.Value); };
            _btnTriangleObj.RightMouseButtonPressed += delegate { DoHotKey(_settingTriangleObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            Image _btnXGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgX,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "X Ground"
            };
            _btnXGnd.LeftMouseButtonPressed += delegate { AddGround(_btnXGnd, _settingXGndBinding.Value); };
            _btnXGnd.RightMouseButtonPressed += delegate { RemoveGround(_settingXGndBinding.Value); };

            Image _btnXObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgX,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "X Object"
            };
            _btnXObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingXObjBinding.Value); };
            _btnXObj.RightMouseButtonPressed += delegate { DoHotKey(_settingXObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            Image _btnClearGnd = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgClear,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(curX, curY),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Clear Ground"
            };
            _btnClearGnd.LeftMouseButtonPressed += delegate { DoHotKey(_settingClearGndBinding.Value); };

            Image _btnClearObj = new Image
            {
                Parent = _cmdPanel,
                Texture = _imgClear,
                Size = new Point(_settingImgWidth.Value, _settingImgWidth.Value),
                Location = new Point(
                    (horizontal ? curX : curX + _settingImgWidth.Value),
                    (horizontal ? curY + _settingImgWidth.Value : curY)
                    ),
                Opacity = _settingOpacity.Value,
                BasicTooltipText = "Clear Object"
            };
            _btnClearObj.LeftMouseButtonPressed += delegate { DoHotKey(_settingClearObjBinding.Value); };

            if (horizontal)
                curX += _settingImgWidth.Value;
            else
                curY += _settingImgWidth.Value;

            if (_settingDrag.Value) {
                Panel dragBox = new Panel() {
                    Parent = _cmdPanel,
                    Location = new Point(0, 0),
                    Size = new Point(_settingImgWidth.Value / 2, _settingImgWidth.Value / 2),
                    BackgroundColor = Color.White,
                    ZIndex = 10,
                };
                dragBox.LeftMouseButtonPressed += delegate {
                    _dragging = true;
                    _dragStart = InputService.Input.Mouse.Position;
                };
                dragBox.LeftMouseButtonReleased += delegate {
                    _dragging = false;
                    _settingLoc.Value = _cmdPanel.Location;
                };
            }
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
            btn.BackgroundColor = Color.Yellow;
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
        }
    }

}
