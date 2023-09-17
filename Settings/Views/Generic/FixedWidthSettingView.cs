using System.Reflection;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings;
using Blish_HUD.Settings.UI.Views;

namespace Manlaan.CommanderMarkers.Settings.Views.Generic;

public class FixedWidthBoolSettingView : BoolSettingView
{
    private readonly int _fixedWidth;

    public FixedWidthBoolSettingView(SettingEntry<bool> setting, int definedWidth = -1) : base(setting, definedWidth)
    {
        _fixedWidth = definedWidth;
    }

    protected override void BuildSetting(Container buildPanel)
    {
        base.BuildSetting(buildPanel);

        if (GetCheckbox() is { } checkbox)
        {
            checkbox.Width = _fixedWidth;
        }
    }

    protected override void RefreshDisplayName(string displayName)
    {
        if (GetCheckbox() is { } checkbox)
        {
            checkbox.Text = displayName;
        }

        // no-op the call to refresh display name, we don't want it to run
        //base.RefreshDisplayName(displayName);
    }

    private Checkbox? GetCheckbox()
    {
        var fieldInfo = typeof(BoolSettingView).GetField("_boolCheckbox", BindingFlags.NonPublic | BindingFlags.Instance);

        return fieldInfo?.GetValue(this) as Checkbox;
    }

    public static IView FromSetting(SettingEntry<bool> setting, int definedWidth = -1)
    {
        return new FixedWidthBoolSettingView(setting, definedWidth);
    }
}