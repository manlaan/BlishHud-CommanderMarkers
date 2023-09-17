using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings.UI.Views;
using Manlaan.CommanderMarkers.Settings.Enums;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class KeybindSettingsView : View
{
    protected SettingService? _settings;
    protected override void Build(Container buildPanel)
    {
        _settings = Service.Settings;

        base.Build(buildPanel);

      /*  var panel = new FlowPanel()
            .BeginFlow(buildPanel, new Point(-95, 0), new Point(0, 5))
            .AddFlowControl(new StandardButton
            {
                Text = "Patch Notes",
                BasicTooltipText = "Open the patch notes in your default web browser"
            }, out var patchNotesButton)
            .AddSpace()
            .AddSpace()
            .AddSpace()

            .AddSpace()
            .AddSpace();

    
        patchNotesButton.Click += (s, e) =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://pkgs.blishhud.com/Manlaan.CommanderMarkers.html",
                UseShellExecute = true
            });
        };*/


        int labelWidth = 60;
        int bindingWidth = 145;


        Panel keysPanel = new Panel()
        {
            CanScroll = false,
            Parent = buildPanel,
            HeightSizingMode = SizingMode.AutoSize,
            Width = 360,
            Location = new Point(10, 10),
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

    }
}