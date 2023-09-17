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

    public void LoadMarkerSet(MarkerSet markerSet, int idx)
    {
        _markerSet = markerSet;
        _updateListingIndex = idx;
        ClearChildren();    

        var metaFlow = new FlowPanel()
        {
            Parent = this,
            FlowDirection = ControlFlowDirection.LeftToRight,
            ControlPadding = new Vector2(10,5),
            Size = new Point(420, 185),
        };
        var export = new StandardButton()
        {
            Parent = metaFlow,
            Text = "Export To Clipboard",
            Width=200,
            BasicTooltipText="Export this marker set to your clipboard to share with others"
        };
        var import = new StandardButton()
        {
            Parent = metaFlow,
            Text = "Import From Clipboard",
            Width=200,
            BasicTooltipText="Copy a marker set to your clipboard, then import it by clicking this button"
        };

        export.Click += (s, e) =>
        {
            string json = JsonConvert.SerializeObject(_markerSet);
            string output = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            Debug.WriteLine(json);
            Debug.WriteLine(output);
            System.Windows.Forms.Clipboard.SetText(json);
            ScreenNotification.ShowNotification($"Marker set {_markerSet.name} copied to your clipboard!", ScreenNotification.NotificationType.Blue, Service.Textures!._blishHeart, 4);
        };
        import.Click += (s, e) =>
        {
            try
            {
                string json = System.Windows.Forms.Clipboard.GetText();
                MarkerSet? markerSet = JsonConvert.DeserializeObject<MarkerSet>(json);
                if(markerSet == null)
                {
                    throw new Exception("Invalid JSON");
                }
                ScreenNotification.ShowNotification($"Imported marker set {markerSet.name}", ScreenNotification.NotificationType.Green, Service.Textures!._blishHeart, 4);
                _markerSet.CloneFromMarkerSet(markerSet);
                LoadMarkerSet(_markerSet,_updateListingIndex);

            }
            catch (Exception)
            {
                ScreenNotification.ShowNotification("Unable to import clipboard content\nDid you copy a marker set first?", ScreenNotification.NotificationType.Red, null, 5);
            }

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
            Text = "Trigger Location: "
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
