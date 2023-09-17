using System;
using System.Reflection;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings;
using Blish_HUD.Settings.UI.Views;

namespace Manlaan.CommanderMarkers.Settings.Views.Generic;

public static class AlignedEnumSettingView
{
    public static IView? FromEnum(SettingEntry setting, int definedWidth = -1)
    {
        return Activator.CreateInstance(typeof(AlignedEnumSettingView<>).MakeGenericType(setting.SettingType), setting, definedWidth) as IView;
    }
}

public class AlignedEnumSettingView<TEnum> : EnumSettingView<TEnum> where TEnum : struct, Enum
{
    protected static int NUMERIC_SLIDER_WIDTH = 277; //Make this match Blish's TRACKBAR_WIDTH in NumericSettingView.cs
    protected static int DROPDOWN_HEIGHT = 27;
    public AlignedEnumSettingView(SettingEntry<TEnum> setting, int definedWidth = -1) : base(setting, definedWidth)
    {
    }

    protected override void BuildSetting(Container buildPanel)
    {
        base.BuildSetting(buildPanel);

        var fieldInfo = typeof(EnumSettingView<TEnum>).GetField("_displayNameLabel", BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo?.GetValue(this) is Label label)
        {
            label.AutoSizeWidth = false;
            label.Width = 175;
        }

        fieldInfo = typeof(EnumSettingView<TEnum>).GetField("_enumDropdown", BindingFlags.NonPublic | BindingFlags.Instance);

        if (fieldInfo?.GetValue(this) is Dropdown dropdown)
        {
            dropdown.Size = new(NUMERIC_SLIDER_WIDTH, DROPDOWN_HEIGHT);
        }
    }
}