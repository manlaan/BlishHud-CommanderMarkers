using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;

namespace Manlaan.CommanderMarkers.Views
{
    class SettingsView : View
    {
        protected override void Build(Container buildPanel) {
            int labelWidth = 60;
            int bindingWidth = 145;

            Panel keysPanel = new Panel() {
                CanScroll = false,
                Parent = buildPanel,
                HeightSizingMode = SizingMode.AutoSize,
                Width = 360,
                Location = new Point(10, 10),
            };
            Panel manualPanel = new Panel() {
                CanScroll = false,
                Parent = buildPanel,
                HeightSizingMode = SizingMode.AutoSize,
                Width = 310,
                Location = new Point(keysPanel.Right + 10, 10),
            };

            #region Keys Panel
            Label settingGround_Label = new Label() {
                Location = new Point(labelWidth + 5, 2),
                Width = bindingWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Ground",
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            Label settingObject_Label = new Label() {
                Location = new Point(settingGround_Label.Right, settingGround_Label.Top),
                Width = bindingWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Object",
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            Label settingArrow_Label = new Label() {
                Location = new Point(0, settingGround_Label.Bottom + 2),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Arrow",
            };
            KeybindingAssigner settingArrowGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingArrowGndBinding.Value,
                Location = new Point(settingArrow_Label.Right + 5, settingArrow_Label.Top - 1),
            };
            settingArrowGnd_Keybind.BindingChanged += delegate {
                Module._settingArrowGndBinding.Value = settingArrowGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingArrowObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingArrowObjBinding.Value,
                Location = new Point(settingArrowGnd_Keybind.Right, settingArrow_Label.Top - 1),
            };
            settingArrowGnd_Keybind.BindingChanged += delegate {
                Module._settingArrowObjBinding.Value = settingArrowObj_Keybind.KeyBinding;
            };

            Label settingCircle_Label = new Label() {
                Location = new Point(0, settingArrow_Label.Bottom),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Circle",
            };
            KeybindingAssigner settingCircleGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingCircleGndBinding.Value,
                Location = new Point(settingCircle_Label.Right + 5, settingCircle_Label.Top - 1),
            };
            settingCircleGnd_Keybind.BindingChanged += delegate {
                Module._settingCircleGndBinding.Value = settingCircleGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingCircleObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingCircleObjBinding.Value,
                Location = new Point(settingCircleGnd_Keybind.Right, settingCircle_Label.Top - 1),
            };
            settingCircleGnd_Keybind.BindingChanged += delegate {
                Module._settingCircleObjBinding.Value = settingCircleObj_Keybind.KeyBinding;
            };

            Label settingHeart_Label = new Label() {
                Location = new Point(0, settingCircle_Label.Bottom),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Heart",
            };
            KeybindingAssigner settingHeartGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingHeartGndBinding.Value,
                Location = new Point(settingHeart_Label.Right + 5, settingHeart_Label.Top - 1),
            };
            settingHeartGnd_Keybind.BindingChanged += delegate {
                Module._settingHeartGndBinding.Value = settingHeartGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingHeartObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingHeartObjBinding.Value,
                Location = new Point(settingHeartGnd_Keybind.Right, settingHeart_Label.Top - 1),
            };
            settingHeartGnd_Keybind.BindingChanged += delegate {
                Module._settingHeartObjBinding.Value = settingHeartObj_Keybind.KeyBinding;
            };

            Label settingSquare_Label = new Label() {
                Location = new Point(0, settingHeart_Label.Bottom),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Square",
            };
            KeybindingAssigner settingSquareGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingSquareGndBinding.Value,
                Location = new Point(settingSquare_Label.Right + 5, settingSquare_Label.Top - 1),
            };
            settingSquareGnd_Keybind.BindingChanged += delegate {
                Module._settingSquareGndBinding.Value = settingSquareGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingSquareObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingSquareObjBinding.Value,
                Location = new Point(settingSquareGnd_Keybind.Right, settingSquare_Label.Top - 1),
            };
            settingSquareGnd_Keybind.BindingChanged += delegate {
                Module._settingSquareObjBinding.Value = settingSquareObj_Keybind.KeyBinding;
            };

            Label settingStar_Label = new Label() {
                Location = new Point(0, settingSquare_Label.Bottom),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Star",
            };
            KeybindingAssigner settingStarGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingStarGndBinding.Value,
                Location = new Point(settingStar_Label.Right + 5, settingStar_Label.Top - 1),
            };
            settingStarGnd_Keybind.BindingChanged += delegate {
                Module._settingStarGndBinding.Value = settingStarGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingStarObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingStarObjBinding.Value,
                Location = new Point(settingStarGnd_Keybind.Right, settingStar_Label.Top - 1),
            };
            settingStarGnd_Keybind.BindingChanged += delegate {
                Module._settingStarObjBinding.Value = settingStarObj_Keybind.KeyBinding;
            };

            Label settingSpiral_Label = new Label() {
                Location = new Point(0, settingStar_Label.Bottom),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Spiral",
            };
            KeybindingAssigner settingSpiralGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingSpiralGndBinding.Value,
                Location = new Point(settingSpiral_Label.Right + 5, settingSpiral_Label.Top - 1),
            };
            settingSpiralGnd_Keybind.BindingChanged += delegate {
                Module._settingSpiralGndBinding.Value = settingSpiralGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingSpiralObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingSpiralObjBinding.Value,
                Location = new Point(settingSpiralGnd_Keybind.Right, settingSpiral_Label.Top - 1),
            };
            settingSpiralGnd_Keybind.BindingChanged += delegate {
                Module._settingSpiralObjBinding.Value = settingSpiralObj_Keybind.KeyBinding;
            };

            Label settingTriangle_Label = new Label() {
                Location = new Point(0, settingSpiral_Label.Bottom),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Triangle",
            };
            KeybindingAssigner settingTriangleGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingTriangleGndBinding.Value,
                Location = new Point(settingTriangle_Label.Right + 5, settingTriangle_Label.Top - 1),
            };
            settingTriangleGnd_Keybind.BindingChanged += delegate {
                Module._settingTriangleGndBinding.Value = settingTriangleGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingTriangleObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingTriangleObjBinding.Value,
                Location = new Point(settingTriangleGnd_Keybind.Right, settingTriangle_Label.Top - 1),
            };
            settingTriangleGnd_Keybind.BindingChanged += delegate {
                Module._settingTriangleObjBinding.Value = settingTriangleObj_Keybind.KeyBinding;
            };

            Label settingX_Label = new Label() {
                Location = new Point(0, settingTriangle_Label.Bottom),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "X",
            };
            KeybindingAssigner settingXGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingXGndBinding.Value,
                Location = new Point(settingX_Label.Right + 5, settingX_Label.Top - 1),
            };
            settingXGnd_Keybind.BindingChanged += delegate {
                Module._settingXGndBinding.Value = settingXGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingXObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingXObjBinding.Value,
                Location = new Point(settingXGnd_Keybind.Right, settingX_Label.Top - 1),
            };
            settingXGnd_Keybind.BindingChanged += delegate {
                Module._settingXObjBinding.Value = settingXObj_Keybind.KeyBinding;
            };

            Label settingClear_Label = new Label() {
                Location = new Point(0, settingX_Label.Bottom),
                Width = labelWidth,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = keysPanel,
                Text = "Clear",
            };
            KeybindingAssigner settingClearGnd_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingClearGndBinding.Value,
                Location = new Point(settingClear_Label.Right + 5, settingClear_Label.Top - 1),
            };
            settingClearGnd_Keybind.BindingChanged += delegate {
                Module._settingClearGndBinding.Value = settingClearGnd_Keybind.KeyBinding;
            };
            KeybindingAssigner settingClearObj_Keybind = new KeybindingAssigner() {
                NameWidth = 0,
                Size = new Point(bindingWidth, 18),
                Parent = keysPanel,
                KeyBinding = Module._settingClearObjBinding.Value,
                Location = new Point(settingClearGnd_Keybind.Right, settingClear_Label.Top - 1),
            };
            settingClearGnd_Keybind.BindingChanged += delegate {
                Module._settingClearObjBinding.Value = settingClearObj_Keybind.KeyBinding;
            };
            #endregion

            #region manual Panel
            Label settingOrientation_Label = new Label() {
                Location = new Point(0, 0),
                Width = 75,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = manualPanel,
                Text = "Orientation",
            };
            Dropdown settingManualOrientation_Select = new Dropdown() {
                Location = new Point(settingOrientation_Label.Right + 5, settingOrientation_Label.Top - 4),
                Width = 100,
                Parent = manualPanel,
            };
            foreach (string s in Module._orientation) {
                settingManualOrientation_Select.Items.Add(s);
            }
            settingManualOrientation_Select.SelectedItem = Module._settingOrientation.Value;
            settingManualOrientation_Select.ValueChanged += delegate {
                Module._settingOrientation.Value = settingManualOrientation_Select.SelectedItem;
            };

            Label settingWidth_Label = new Label() {
                Location = new Point(0, settingOrientation_Label.Bottom + 6),
                Width = 75,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = manualPanel,
                Text = "Icon Width: ",
            };
            TrackBar settingImgWidth_Slider = new TrackBar() {
                Location = new Point(settingWidth_Label.Right + 5, settingWidth_Label.Top),
                Width = 220,
                MaxValue = 200,
                MinValue = 0,
                Value = Module._settingImgWidth.Value,
                Parent = manualPanel,
            };
            settingImgWidth_Slider.ValueChanged += delegate { Module._settingImgWidth.Value = (int)settingImgWidth_Slider.Value; };

            Label settingOpacity_Label = new Label() {
                Location = new Point(0, settingWidth_Label.Bottom + 6),
                Width = 75,
                AutoSizeHeight = false,
                WrapText = false,
                Parent = manualPanel,
                Text = "Opacity: ",
            };
            TrackBar settingOpacity_Slider = new TrackBar() {
                Location = new Point(settingOpacity_Label.Right + 5, settingOpacity_Label.Top),
                Width = 220,
                MaxValue = 100,
                MinValue = 0,
                Value = Module._settingOpacity.Value * 100,
                Parent = manualPanel,
            };
            settingOpacity_Slider.ValueChanged += delegate { Module._settingOpacity.Value = settingOpacity_Slider.Value / 100; };

            IView settingClockDrag_View = SettingView.FromType(Module._settingDrag, buildPanel.Width);
            ViewContainer settingClockDrag_Container = new ViewContainer() {
                WidthSizingMode = SizingMode.Fill,
                Location = new Point(0, settingOpacity_Label.Bottom + 3),
                Parent = manualPanel
            };
            settingClockDrag_Container.Show(settingClockDrag_View);
            #endregion
        }

    }
}
