using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD.Settings.UI.Views;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Settings.Enums;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Utils;
using System.Diagnostics;
using Blish_HUD;
using Blish_HUD.Content;

namespace Manlaan.CommanderMarkers.Settings.Views;

class ModuleSettingsView : View
{
    protected SettingService _settings;
    public ModuleSettingsView(SettingService settings) {
        this._settings = settings;
    }
    protected override void Build(Container buildPanel)
    {
        buildPanel.AddControl(new StandardButton
        {
            Parent = buildPanel,
            Text = "Open Settings",
            Size = buildPanel.Size.Scale(0.20f),
            Location = buildPanel.Size.Half() - buildPanel.Size.Scale(0.20f).Half(),

        }, out var btn).Click += (_, _) => Service.SettingsWindow?.Show();

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
