using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Presets.Model;
using Microsoft.Xna.Framework;
using System;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class MarkerEditor : FlowPanel
{
    Action<MarkerEditor> _onDeleteCallback;
    private PositionFields _position;

    private Dropdown _iconPicker;

    private MarkerCoord _markerCoord;

    public MarkerCoord Marker { get => _markerCoord; }

    public MarkerEditor(MarkerCoord marker, Action<MarkerEditor> onDeleteCallback) : base()
    {
        _markerCoord = marker;

        _onDeleteCallback = onDeleteCallback;

        FlowDirection = ControlFlowDirection.LeftToRight;
        ControlPadding = new Vector2(10, 5);
        OuterControlPadding = new Vector2(0, 10);
        Size = new(450, 70);



        _iconPicker = new Dropdown()
        {
            Parent = this,
            //Location = new Point(0, 0)
            Size = new(100, 30)
        };

        

        foreach(var m  in Enum.GetValues(typeof(SquadMarker)))
        {
            _iconPicker.Items.Add(Enum.GetName(typeof(SquadMarker), m));
        }
        SetIconPicker(marker.icon);
        _iconPicker.ValueChanged += IconPicker_ValueChanged;

        var description = new TextBox()
        {
            Parent = this,
            Text = marker.name,
            Size = new Point(300,30)
        };

        var deleteButton = new GlowButton()
        {
            Parent = this,
            BasicTooltipText = "Delete marker",
            Icon = Service.Textures!._imgClear,
            Size = new Point(28, 28),
            
        };
        deleteButton.Click += (s, e) => _onDeleteCallback(this);

        _position = new PositionFields(marker)
        {
            Parent = this,
        };
        _position.WorldCoordChanged += (s, e) => marker.FromWorldCoord(e);


    }

    private void SetIconPicker(int icon)
    {
        var text = Enum.GetName(typeof(SquadMarker), icon);

        _iconPicker.SelectedItem = text;
    }

    private void IconPicker_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        _markerCoord.icon = (int)SquadMarker.None.EnumValue(e.CurrentValue);
    }
    protected override void DisposeControl()
    {
        _iconPicker.ValueChanged -= IconPicker_ValueChanged;  
    }
}
