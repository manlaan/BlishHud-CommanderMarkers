using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Presets.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class MarkerEditor : FlowPanel
{
    Action<MarkerEditor> _onDeleteCallback;
    private PositionFields? _position;

    private MarkerCoord? _markerCoord;

    private IconPicker? _iconPicker;

    public MarkerCoord Marker { get => _markerCoord ?? new MarkerCoord(); }

    public MarkerEditor(MarkerCoord marker, Action<MarkerEditor> onDeleteCallback) : base()
    {
        _markerCoord = marker;

        _onDeleteCallback = onDeleteCallback;

        FlowDirection = ControlFlowDirection.LeftToRight;
        ControlPadding = new Vector2(10, 5);
        OuterControlPadding = new Vector2(0, 10);
        Size = new(450, 70);


        _iconPicker = new IconPicker()
        {
            Parent = this,
            Size = new Point(300, 30)
        };
        List<(int, Texture2D)> list = new()
        {
            (1, Service.Textures!._imgArrow),
            (2, Service.Textures!._imgCircle),
            (3, Service.Textures!._imgHeart),
            (4, Service.Textures!._imgSquare),
            (5, Service.Textures!._imgStar),
            (6, Service.Textures!._imgSpiral),
            (7, Service.Textures!._imgTriangle),
            (8, Service.Textures!._imgX),
        };
        _iconPicker.LoadList(list);
        _iconPicker.SelectItem(marker.icon);
        _iconPicker.IconSelectionChanged += (s, e) =>
        {
            _markerCoord.icon = e;
        };

        var description = new TextBox()
        {
            Parent = this,
            Text = marker.name,
            Size = new Point(100,30),
            BasicTooltipText = "Name the marker.\nHelpful for remembering which marker is where."
        };

        var deleteButton = new Image()
        {
            Parent = this,
            BasicTooltipText = "Delete marker",
            Texture = Service.Textures!.IconDelete,
            Size = new Point(28, 28),   
            
        };
        deleteButton.Click += (s, e) => _onDeleteCallback(this);

        _position = new PositionFields(marker)
        {
            Parent = this,
        };
        _position.WorldCoordChanged += (s, e) => marker.FromWorldCoord(e);


    }

    protected override void DisposeControl()
    {
        _iconPicker?.Dispose();
        _position?.Dispose();
    }
}
