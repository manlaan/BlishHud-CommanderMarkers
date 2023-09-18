using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Presets.Model;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class MarkerSetEditor : FlowPanel
{
    protected int _updateListingIndex = -1;
    Action<bool> _returnToList;

    protected MarkerSet _markerSet = new();

    public MarkerSet MarkerSet { get => _markerSet; }
    public MarkerSetEditor(Action<bool> callback) : base()
    {
        ControlPadding = new Vector2(5, 5);
        _returnToList = callback;
    }

    public void LoadMarkerSet(MarkerSet? markerSet, int idx)
    {
        _markerSet = markerSet ?? new MarkerSet();
        _updateListingIndex = idx;
        ClearChildren();    

        var metaFlow = new FlowPanel()
        {
            Parent = this,
            FlowDirection = ControlFlowDirection.LeftToRight,
            ControlPadding = new Vector2(10,5),
            Size = new Point(420, 155),
        };
        
        new Label()
        {
            Parent = metaFlow,
            Text = "Set Name:",
            Size = new Point(100, 30),
            BasicTooltipText = "The name shown on the map when you are within range of using the marker set"

        };
        var title = new TextBox()
        {
            Parent = metaFlow,
            Location = new Point(0, 0),
            Size = new Point(300, 30),
            Text = _markerSet.name,
            BasicTooltipText = "The name shown on the map when you are within range of using the marker set"

        };
        title.TextChanged += (s, e) => _markerSet.name = title.Text;

        new Label()
        {
            Parent = metaFlow,
            Text = "Description",
            Size = new Point(100, 30),
            BasicTooltipText = "This text is shown on the map when you are within range of using the marker set"
        };
       
        var description = new TextBox()
        {
            Parent = metaFlow,
            Location = new Point(0, 0),
            Size = new Point(300, 30),
            Text = _markerSet.description,
            BasicTooltipText = "This text is shown on the map when you are within range of using the marker set"

        };
        description.TextChanged += (s,e) => _markerSet.description = description.Text;

        new Label()
        {
            Parent = metaFlow,
            Size = new Point(100, 30),
            Text = "Trigger Location: ",
            BasicTooltipText = "Location to be near to activate this marker set"
        };
        var label = new Label()
        {
            Parent = metaFlow,
            Size = new Point(300, 30),
            Text = $"Map: {Service.MapDataCache.Describe(_markerSet.MapId)},  API Id: {_markerSet.MapId}",
            BasicTooltipText ="Set trigger location to update map"
        };
        var triggerFields = new PositionFields(markerSet.Trigger)
        {
            Parent = metaFlow,
        };
        triggerFields.WorldCoordChanged += (s, e) =>
        {
            _markerSet.trigger = e;
            _markerSet.mapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
            label.Text = $"Map: {Service.MapDataCache.Describe(_markerSet.MapId)},  API Id: {_markerSet.MapId}";
        };

        var AddMarkerButton = new StandardButton()
        {
            Parent = this,
            Text = "Add Marker",
            Width = 410
        };

        markerSet.marks.ForEach( mark =>
        {
            new MarkerEditor(mark, RemoveMarker)
            {
                Parent = this,
            };
        });


        AddMarkerButton.Click += (s, e) =>
        {
            var marker = new MarkerCoord();
            marker.SetFromMumbleLocation();
            markerSet.marks.Add(marker);
            new MarkerEditor(marker, RemoveMarker) { Parent = this };
        };
    }

    protected void RemoveMarker(MarkerEditor editor)
    {
        Children.Remove(editor);
        _markerSet.marks.Remove(editor.Marker);
        Invalidate();
    }

 }
