using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Blish_HUD.Settings.UI.Views;
using Manlaan.CommanderMarkers.Settings.Enums;
using Manlaan.CommanderMarkers.Settings.Views.Generic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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
        foreach (var image in panel.Children)
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
    public static void VisiblityChanged(this FlowPanel panel, SettingEntry<bool> setting)
    {
        setting.SettingChanged += (_, e) =>
        {
            panel.Visible = e.NewValue;
            panel.Parent?.Invalidate();
        };

        panel.Visible = setting.Value;
        panel.Parent?.Invalidate();
    }

    public static void InvertedVisiblityChanged(this FlowPanel panel, SettingEntry<bool> setting)
    {
        setting.SettingChanged += (_, e) =>
        {
            panel.Visible = !e.NewValue;
            panel.Parent?.Invalidate();
        };

        panel.Visible = !setting.Value;
        panel.Parent?.Invalidate();
    }

    public static void LayoutChange(this FlowPanel panel, SettingEntry<Layout> setting, int nestingLevel = 0)
    {
        setting.SettingChanged += (_, e) =>
        {
            panel.FlowDirection = GetFlowDirection(e.NewValue, nestingLevel);
        };

        panel.FlowDirection = GetFlowDirection(setting.Value, nestingLevel);
    }


    private static ControlFlowDirection GetFlowDirection(Layout orientation, int nestingLevel = 0)
    {
        // FlowDirection based on Orientation 
        // V             H 1  2  3 L>R
        // T  1 B1 B2 B3   B1 B1 B1
        // v  2 B1 B2 B3   B2 B2 B2
        // B  3 B1 B2 B3   B3 B3 B3

        var isChild = nestingLevel % 2 is not 0;

        return orientation switch
        {
            Layout.Vertical when isChild => ControlFlowDirection.SingleTopToBottom,
            Layout.Horizontal when isChild => ControlFlowDirection.SingleLeftToRight,
            //Layout.SingleRow when isChild => ControlFlowDirection.SingleLeftToRight,
            //Layout.SingleColumn when isChild => ControlFlowDirection.SingleTopToBottom,
            _ when isChild => ControlFlowDirection.SingleLeftToRight,

            Layout.Vertical => ControlFlowDirection.SingleLeftToRight,
            Layout.Horizontal => ControlFlowDirection.SingleTopToBottom,
            //Layout.SingleRow => ControlFlowDirection.SingleLeftToRight,
            //Layout.SingleColumn => ControlFlowDirection.SingleTopToBottom,
            _ => ControlFlowDirection.SingleTopToBottom,
        };
    }

    public static FlowPanel BeginFlow(this FlowPanel panel, Container parent, Point sizeOffset, Point locationOffset)
    {
        panel.FlowDirection = ControlFlowDirection.SingleTopToBottom;
        panel.OuterControlPadding = new Vector2(20, 25);
        panel.Parent = parent;
        panel.Size = parent.Size + sizeOffset;
        panel.ShowBorder = true;
        panel.Location += locationOffset;

        return panel;
    }

    public static FlowPanel BeginFlow(this FlowPanel panel, Container parent)
    {
        return BeginFlow(panel, parent, new Point(0), new Point(0));
    }

    public static FlowPanel AddSetting(this FlowPanel panel, SettingEntry setting)
    {
        var viewContainer = new ViewContainer
        {
            Parent = panel,
        };

        if (setting.SettingType == typeof(bool))
        {
            viewContainer.Show(FixedWidthBoolSettingView.FromSetting((SettingEntry<bool>)setting, panel.Width));
        }
        else
        {
            viewContainer.Show(SettingView.FromType(setting, panel.Width));
        }

        return panel;
    }

    public static FlowPanel AddSetting(this FlowPanel panel, IEnumerable<SettingEntry>? settings)
    {
        if (settings is null) return panel;

        foreach (var setting in settings)
        {
            panel.AddSetting(setting);
        }
        return panel;
    }

    public static FlowPanel AddSettingEnum(this FlowPanel panel, SettingEntry enumSetting)
    {
        var viewContainer = new ViewContainer { Parent = panel };
        viewContainer.Show(AlignedEnumSettingView.FromEnum(enumSetting, panel.Width));
        //        viewContainer.Show(CustomEnumSettingView.FromEnum(enumSetting, panel.Width));

        return panel;
    }






    public static FlowPanel AddSpace(this FlowPanel panel)
    {
        var _ = new ViewContainer
        {
            Parent = panel,
        };
        return panel;
    }

    public static FlowPanel AddString(this FlowPanel panel, string text)
    {
        var _ = new Label
        {
            Parent = panel,
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            Text = text,
            WrapText = false,
            Location = new Point(25, 0),
        };

        return panel;
    }
    public static FlowPanel AddChildPanel(this FlowPanel panel, Panel child)
    {
        child.Parent = panel;

        return panel;
    }
    public static FlowPanel Indent(this FlowPanel panel)
    {
        panel.Left = 30;
        return panel;
    }

    public static FlowPanel AddFlowControl(this FlowPanel panel, Control control, out Control generatedControl)
    {
        control.Parent = panel;

        panel.AddChild(control);
        generatedControl = control;
        return panel;
    }

    public static FlowPanel AddFlowControl(this FlowPanel panel, Control control)
    {
        control.Parent = panel;

        panel.AddChild(control);
        return panel;
    }
}
