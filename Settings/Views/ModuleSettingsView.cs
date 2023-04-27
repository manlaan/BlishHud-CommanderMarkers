using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Settings.Enums;
using Blish_HUD.Input;
using Blish_HUD.Settings;

namespace Manlaan.CommanderMarkers.Settings.Views;

class ModuleSettingsView : View
{
    protected SettingService _settings;
    public ModuleSettingsView(SettingService settings) {
        this._settings = settings;
    }
    protected override void Build(Container buildPanel)
    {
        int labelWidth = 60;
        int bindingWidth = 145;

        Image blishHeart = new Image(Service.Textures!._blishHeart)
        {
            Parent= buildPanel,
            Width = 128,
            Height = 128,
            Location = new Point(buildPanel.Width - 128, buildPanel.Height - 128),
        };
        

        Panel keysPanel = new Panel()
        {
            CanScroll = false,
            Parent = buildPanel,
            HeightSizingMode = SizingMode.AutoSize,
            Width = 360,
            Location = new Point(10, 10),
        };
        Panel manualPanel = new Panel()
        {
            CanScroll = false,
            Parent = buildPanel,
            HeightSizingMode = SizingMode.AutoSize,
            Width = 310,
            Location = new Point(keysPanel.Right + 10, 10),
        };

        #region Keys Panel
        Label settingGround_Label = new Label()
        {
            Location = new Point(labelWidth + 5, 2),
            Width = bindingWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Ground",
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        Label settingObject_Label = new Label()
        {
            Location = new Point(settingGround_Label.Right, settingGround_Label.Top),
            Width = bindingWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Object",
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        Label settingArrow_Label = new Label()
        {
            Location = new Point(0, settingGround_Label.Bottom + 2),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Arrow",
        };
        KeybindingAssigner settingArrowGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingArrowGndBinding.Value,
            Location = new Point(settingArrow_Label.Right + 5, settingArrow_Label.Top - 1),
        };
        settingArrowGnd_Keybind.BindingChanged += delegate {
            _settings._settingArrowGndBinding.Value = settingArrowGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingArrowObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingArrowObjBinding.Value,
            Location = new Point(settingArrowGnd_Keybind.Right, settingArrow_Label.Top - 1),
        };
        settingArrowGnd_Keybind.BindingChanged += delegate {
            _settings._settingArrowObjBinding.Value = settingArrowObj_Keybind.KeyBinding;
        };

        Label settingCircle_Label = new Label()
        {
            Location = new Point(0, settingArrow_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Circle",
        };
        KeybindingAssigner settingCircleGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingCircleGndBinding.Value,
            Location = new Point(settingCircle_Label.Right + 5, settingCircle_Label.Top - 1),
        };
        settingCircleGnd_Keybind.BindingChanged += delegate {
            _settings._settingCircleGndBinding.Value = settingCircleGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingCircleObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingCircleObjBinding.Value,
            Location = new Point(settingCircleGnd_Keybind.Right, settingCircle_Label.Top - 1),
        };
        settingCircleGnd_Keybind.BindingChanged += delegate {
            _settings._settingCircleObjBinding.Value = settingCircleObj_Keybind.KeyBinding;
        };

        Label settingHeart_Label = new Label()
        {
            Location = new Point(0, settingCircle_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Heart",
        };
        KeybindingAssigner settingHeartGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingHeartGndBinding.Value,
            Location = new Point(settingHeart_Label.Right + 5, settingHeart_Label.Top - 1),
        };
        settingHeartGnd_Keybind.BindingChanged += delegate {
            _settings._settingHeartGndBinding.Value = settingHeartGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingHeartObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingHeartObjBinding.Value,
            Location = new Point(settingHeartGnd_Keybind.Right, settingHeart_Label.Top - 1),
        };
        settingHeartGnd_Keybind.BindingChanged += delegate {
            _settings._settingHeartObjBinding.Value = settingHeartObj_Keybind.KeyBinding;
        };

        Label settingSquare_Label = new Label()
        {
            Location = new Point(0, settingHeart_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Square",
        };
        KeybindingAssigner settingSquareGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingSquareGndBinding.Value,
            Location = new Point(settingSquare_Label.Right + 5, settingSquare_Label.Top - 1),
        };
        settingSquareGnd_Keybind.BindingChanged += delegate {
            _settings._settingSquareGndBinding.Value = settingSquareGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingSquareObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingSquareObjBinding.Value,
            Location = new Point(settingSquareGnd_Keybind.Right, settingSquare_Label.Top - 1),
        };
        settingSquareGnd_Keybind.BindingChanged += delegate {
            _settings._settingSquareObjBinding.Value = settingSquareObj_Keybind.KeyBinding;
        };

        Label settingStar_Label = new Label()
        {
            Location = new Point(0, settingSquare_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Star",
        };
        KeybindingAssigner settingStarGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingStarGndBinding.Value,
            Location = new Point(settingStar_Label.Right + 5, settingStar_Label.Top - 1),
        };
        settingStarGnd_Keybind.BindingChanged += delegate {
            _settings._settingStarGndBinding.Value = settingStarGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingStarObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingStarObjBinding.Value,
            Location = new Point(settingStarGnd_Keybind.Right, settingStar_Label.Top - 1),
        };
        settingStarGnd_Keybind.BindingChanged += delegate {
            _settings._settingStarObjBinding.Value = settingStarObj_Keybind.KeyBinding;
        };

        Label settingSpiral_Label = new Label()
        {
            Location = new Point(0, settingStar_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Spiral",
        };
        KeybindingAssigner settingSpiralGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingSpiralGndBinding.Value,
            Location = new Point(settingSpiral_Label.Right + 5, settingSpiral_Label.Top - 1),
        };
        settingSpiralGnd_Keybind.BindingChanged += delegate {
            _settings._settingSpiralGndBinding.Value = settingSpiralGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingSpiralObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingSpiralObjBinding.Value,
            Location = new Point(settingSpiralGnd_Keybind.Right, settingSpiral_Label.Top - 1),
        };
        settingSpiralGnd_Keybind.BindingChanged += delegate {
            _settings._settingSpiralObjBinding.Value = settingSpiralObj_Keybind.KeyBinding;
        };

        Label settingTriangle_Label = new Label()
        {
            Location = new Point(0, settingSpiral_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Triangle",
        };
        KeybindingAssigner settingTriangleGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingTriangleGndBinding.Value,
            Location = new Point(settingTriangle_Label.Right + 5, settingTriangle_Label.Top - 1),
        };
        settingTriangleGnd_Keybind.BindingChanged += delegate {
            _settings._settingTriangleGndBinding.Value = settingTriangleGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingTriangleObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingTriangleObjBinding.Value,
            Location = new Point(settingTriangleGnd_Keybind.Right, settingTriangle_Label.Top - 1),
        };
        settingTriangleGnd_Keybind.BindingChanged += delegate {
            _settings._settingTriangleObjBinding.Value = settingTriangleObj_Keybind.KeyBinding;
        };

        Label settingX_Label = new Label()
        {
            Location = new Point(0, settingTriangle_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "X",
        };
        KeybindingAssigner settingXGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingXGndBinding.Value,
            Location = new Point(settingX_Label.Right + 5, settingX_Label.Top - 1),
        };
        settingXGnd_Keybind.BindingChanged += delegate {
            _settings._settingXGndBinding.Value = settingXGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingXObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingXObjBinding.Value,
            Location = new Point(settingXGnd_Keybind.Right, settingX_Label.Top - 1),
        };
        settingXGnd_Keybind.BindingChanged += delegate {
            _settings._settingXObjBinding.Value = settingXObj_Keybind.KeyBinding;
        };

        Label settingClear_Label = new Label()
        {
            Location = new Point(0, settingX_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Clear",
        };
        KeybindingAssigner settingClearGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingClearGndBinding.Value,
            Location = new Point(settingClear_Label.Right + 5, settingClear_Label.Top - 1),
        };
        settingClearGnd_Keybind.BindingChanged += delegate {
            _settings._settingClearGndBinding.Value = settingClearGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingClearObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingClearObjBinding.Value,
            Location = new Point(settingClearGnd_Keybind.Right, settingClear_Label.Top - 1),
        };
        settingClearGnd_Keybind.BindingChanged += delegate {
            _settings._settingClearObjBinding.Value = settingClearObj_Keybind.KeyBinding;
        };


        Label settingInteract_Label = new Label()
        {
            Location = new Point(0, settingClear_Label.Bottom + settingClear_Label.Height),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Interact",
            BasicTooltipText = "The In-Game keybind for 'interact' (Default F)"
        };
        KeybindingAssigner settingInteract_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingInteractKeyBinding.Value,
            Location = new Point(settingInteract_Label.Right + 5, settingInteract_Label.Top - 1),
            BasicTooltipText = "The In-Game keybind for 'interact' (Default F)"
        };
        settingInteract_Keybind.BindingChanged += delegate {
            _settings._settingInteractKeyBinding.Value = settingInteract_Keybind.KeyBinding;
        };


        #endregion

        #region manual Panel

        IView settingMarkersVisibleView = SettingView.FromType(_settings._settingShowMarkersPanel, buildPanel.Width);
        ViewContainer settingMarkersVisibleContainer = new ViewContainer()
        {
            WidthSizingMode = SizingMode.Fill,
            Location = new Point(0, 0),
            Parent = manualPanel
        };
        settingMarkersVisibleContainer.Show(settingMarkersVisibleView);

        IView settingClickDrag_View = SettingView.FromType(_settings._settingDrag, buildPanel.Width);
        ViewContainer settingClickDrag_Container = new ViewContainer()
        {
            WidthSizingMode = SizingMode.Fill,
            Location = new Point((manualPanel.Width/2)+5, 0),
            Parent = manualPanel
        };
        settingClickDrag_Container.Show(settingClickDrag_View);

        Label settingOrientation_Label = new Label()
        {
            Location = new Point(0, settingMarkersVisibleContainer.Bottom + 6),
            Width = 75,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = manualPanel,
            Text = "Orientation",
        };
        Dropdown settingManualOrientation_Select = new Dropdown()
        {
            Location = new Point(settingOrientation_Label.Right + 5, settingOrientation_Label.Top - 4),
            Width = 100,
            Parent = manualPanel,
        };
        settingManualOrientation_Select.Items.Add(Layout.Horizontal.ToString());
        settingManualOrientation_Select.Items.Add(Layout.Vertical.ToString());

        settingManualOrientation_Select.SelectedItem = _settings._settingOrientation.Value;
        settingManualOrientation_Select.ValueChanged += delegate {
            _settings._settingOrientation.Value = settingManualOrientation_Select.SelectedItem;
        };

        Label settingWidth_Label = new Label()
        {
            Location = new Point(0, settingOrientation_Label.Bottom + 6),
            Width = 75,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = manualPanel,
            Text = "Icon Width: ",
        };
        TrackBar settingImgWidth_Slider = new TrackBar()
        {
            Location = new Point(settingWidth_Label.Right + 5, settingWidth_Label.Top),
            Width = 220,
            MaxValue = 200,
            MinValue = 16,
            Value = _settings._settingImgWidth.Value,
            Parent = manualPanel,
        };
        settingImgWidth_Slider.ValueChanged += delegate { _settings._settingImgWidth.Value = (int)settingImgWidth_Slider.Value; };

        Label settingOpacity_Label = new Label()
        {
            Location = new Point(0, settingWidth_Label.Bottom + 6),
            Width = 75,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = manualPanel,
            Text = "Opacity: ",
        };
        TrackBar settingOpacity_Slider = new TrackBar()
        {
            Location = new Point(settingOpacity_Label.Right + 5, settingOpacity_Label.Top),
            Width = 220,
            MaxValue = 100,
            MinValue = 10,
            Value = _settings._settingOpacity.Value * 100,
            Parent = manualPanel,
        };
        settingOpacity_Slider.ValueChanged += delegate { _settings._settingOpacity.Value = settingOpacity_Slider.Value / 100; };

        

        IView settingOnlyComm_View = SettingView.FromType(_settings._settingOnlyWhenCommander, buildPanel.Width);
        ViewContainer settingOnlyComm_Container = new ViewContainer()
        {
            WidthSizingMode = SizingMode.Fill,
            Location = new Point(0, settingOpacity_Label.Bottom + 15),
            Parent = manualPanel
        };
        settingOnlyComm_Container.Show(settingOnlyComm_View);

        Label settingMarkerDelay_Label = new Label()
        {
            Location = new Point(0, settingOnlyComm_Container.Bottom + 6),
            Width = 75,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = manualPanel,
            Text = "Delay: ",
            BasicTooltipText = "Delay in milliseconds to wait between marker placement\nFaster <-----> Slower"
        };
        TrackBar settingMarkerDelay_Slider = new TrackBar()
        {
            Location = new Point(settingMarkerDelay_Label.Right + 5, settingMarkerDelay_Label.Top),
            Width = 220,
            MaxValue = 300,
            MinValue = 50,
            Value = _settings._settingMarkerPlaceDelay.Value,
            Parent = manualPanel,
            BasicTooltipText = "Delay in milliseconds to wait between marker placement\nFaster <-----> Slower"

        };
        settingMarkerDelay_Slider.ValueChanged += delegate { _settings._settingMarkerPlaceDelay.Value = (int)settingMarkerDelay_Slider.Value; };

        /*IView settingMapVisible_View = SettingView.FromType(_settings._settingMapVisible, buildPanel.Width);
        ViewContainer settingMapVisible_Container = new ViewContainer()
        {
            WidthSizingMode = SizingMode.Fill,
            Location = new Point(0, settingOnlyComm_Container.Bottom + 3),
            Parent = manualPanel
        };
        settingMapVisible_Container.Show(settingMapVisible_View);*/
        #endregion
    }


    protected void CreateDoubleKeybind(Container buildPanel, string label, SettingEntry<KeyBinding> groundSetting, SettingEntry<KeyBinding> objectSetting)
    {
       /* Label settingCircle_Label = new Label()
        {
            Location = new Point(0, settingArrow_Label.Bottom),
            Width = labelWidth,
            AutoSizeHeight = false,
            WrapText = false,
            Parent = keysPanel,
            Text = "Circle",
        };
        KeybindingAssigner settingCircleGnd_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingCircleGndBinding.Value,
            Location = new Point(settingCircle_Label.Right + 5, settingCircle_Label.Top - 1),
        };
        settingCircleGnd_Keybind.BindingChanged += delegate {
            _settings._settingCircleGndBinding.Value = settingCircleGnd_Keybind.KeyBinding;
        };
        KeybindingAssigner settingCircleObj_Keybind = new KeybindingAssigner()
        {
            NameWidth = 0,
            Size = new Point(bindingWidth, 18),
            Parent = keysPanel,
            KeyBinding = _settings._settingCircleObjBinding.Value,
            Location = new Point(settingCircleGnd_Keybind.Right, settingCircle_Label.Top - 1),
        };
        settingCircleGnd_Keybind.BindingChanged += delegate {
            _settings._settingCircleObjBinding.Value = settingCircleObj_Keybind.KeyBinding;
        };*/
    }

}
