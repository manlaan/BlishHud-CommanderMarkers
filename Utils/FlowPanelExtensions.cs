using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Settings.Enums;
using Manlaan.CommanderMarkers.Settings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Utils;

public static class FlowPanelExtensions
{

    public static void LayoutChange(this FlowPanel panel, SettingEntry<string> setting, int nestingLevel = 0)
    {
        setting.SettingChanged += (_, e) =>
        {
            panel.FlowDirection = GetFlowDirection(e.NewValue, nestingLevel);
        };

        panel.FlowDirection = GetFlowDirection(setting.Value, nestingLevel);
    }

    private static ControlFlowDirection GetFlowDirection(string orientation, int nestingLevel = 0)
    {
        // FlowDirection based on Orientation 
        // V             H 1  2  3 L>R
        // T  1 B1 B2 B3   B1 B1 B1
        // v  2 B1 B2 B3   B2 B2 B2
        // B  3 B1 B2 B3   B3 B3 B3

        var isChild = nestingLevel % 2 is not 0;

        return orientation switch
        {
            "Vertical" when isChild => ControlFlowDirection.SingleTopToBottom,
            "Horizontal" when isChild => ControlFlowDirection.SingleLeftToRight,
            _ when isChild => ControlFlowDirection.SingleLeftToRight,

            "Vertical" => ControlFlowDirection.SingleLeftToRight,
            "Horizontal" => ControlFlowDirection.SingleTopToBottom,
            _ => ControlFlowDirection.SingleTopToBottom,
        };
    }

    public static void SizeChange(this FlowPanel panel, SettingEntry<int> setting)
    {
        setting.SettingChanged += (_, e) =>
        {
            ResizeImageChildren(panel, e.NewValue);
        };
        ResizeImageChildren(panel, setting.Value);

    }
    private static void ResizeImageChildren(FlowPanel panel, int size)
    {
        foreach( var image in panel.Children)
        {
            if (image.GetType() == typeof(Image))
            {
                image.Size = new(size, size);
            }
        }
        panel.Invalidate();
    }

    public static void OpacityChange(this FlowPanel panel, SettingEntry<float> setting)
    {
        setting.SettingChanged += (_, e) =>
        {
            panel.Opacity = e.NewValue;
        };
        panel.Opacity = setting.Value;

    }
}
