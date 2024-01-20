using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Presets.Model;
using Microsoft.Xna.Framework;
using NAudio.CoreAudioApi;
using System;
using System.Runtime.InteropServices;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class MarkerSetEditor : FlowPanel
{
    protected int _updateListingIndex = -1;
    Action<bool> _returnToList;

    protected MarkerSet _markerSet = new();
    protected StandardButton? _AddMarkerButton;

    private Checkbox? _debug;

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
            Size = new Point(450, 135),
            BackgroundColor=Color.Blue
        };
        
        new Label()
        {
            Parent = metaFlow,
            Text = "Name",
            Size = new Point(99, 30),
            BasicTooltipText = "The name shown on the map when you are within range of using the marker set"

        };
        var title = new TextBox()
        {
            Parent = metaFlow,
            Location = new Point(0, 0),
            Size = new Point(299, 30),
            Text = _markerSet.name,
            BasicTooltipText = "The name shown on the map when you are within range of using the marker set"

        };
        title.TextChanged += (s, e) => _markerSet.name = title.Text;

        var Preview = new IconButton()
        {
            Parent = metaFlow,
            Icon = Service.Textures!.IconEye,
            BasicTooltipText = "Preview",
            Size = new Point(30, 30)
        };
        Preview.MouseEntered += (s, e) => Service.MapWatch.PreviewMarkerSet(_markerSet);
        Preview.MouseLeft += (s, e) => Service.MapWatch.RemovePreviewMarkerSet();

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
        BackgroundColor = Color.Pink;
        new Label()
        {
            Parent = metaFlow,
            Size = new Point(100, 30),
            Text = "Trigger Location",
            BasicTooltipText = "Location to be near to activate this marker set"
        };
        var label = new Label()
        {
            Parent = metaFlow,
            Size = new Point(300, 30),
            Text = $"Map: {Service.MapDataCache.Describe(_markerSet.MapId)}",
            BasicTooltipText ="Set trigger location to update map"
        };
        var triggerFields = new PositionFields(markerSet.Trigger)
        {
            Parent = metaFlow,
            BackgroundColor = Color.Orange,
        };
        triggerFields.WorldCoordChanged += (s, e) =>
        {
            _markerSet.trigger = e;
            _markerSet.mapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
            label.Text = $"Map: {Service.MapDataCache.Describe(_markerSet.MapId)}";
        };
        var flowRow = new Panel()
        {
            Parent = this,
            Width = metaFlow.Width,
            HeightSizingMode=SizingMode.AutoSize
            
        };
        var debugging = Service.Settings.DebugMode.Value;
        _AddMarkerButton = new StandardButton()
        {
            Parent = flowRow,
            Text = "Add Marker",
            Location=new Point(0,0),
            Width = 410,
            Enabled = _markerSet.marks.Count < 8
        };
        
        if (debugging)
        {
            if (_debug == null)
            {
                _debug = new Checkbox()
                {
                    Parent = flowRow,
                    BasicTooltipText = "Debug mode - every Arrow keybind use will add a marker to the list\nat the mouse's world coords",
                    Enabled = true,
                    Checked = false,
                    Location = new Point(_AddMarkerButton.Right + 2, _AddMarkerButton.Top+5),
                    BackgroundColor = Color.White,
                };
                _debug.CheckedChanged += _debug_CheckedChanged;
            }
            else
            {
                _debug.Parent = flowRow;
            }

        }

        markerSet.marks.ForEach( mark =>
        {
            new MarkerEditor(mark, RemoveMarker)
            {
                Parent = this,
            };
        });


        _AddMarkerButton.Click += (s, e) =>
        {
            if(_markerSet.marks.Count < 8)
            {
                var marker = new MarkerCoord();
                marker.SetFromMumbleLocation();
                _markerSet.marks.Add(marker);
                new MarkerEditor(marker, RemoveMarker) { Parent = this };
            }
            _AddMarkerButton.Enabled = _markerSet.marks.Count < 8;
            
        };
    }
    public void DisableDebug()
    {
        if (_debug!=null && _debug.Checked)
        {
            _debug.Checked = false;
        }
    }

    private void _debug_CheckedChanged(object sender, CheckChangedEvent e)
    {
        try
        {
            if (e.Checked)
            {
                Service.Settings._settingArrowGndBinding.Value.Enabled = true;
                Service.Settings._settingArrowGndBinding.Value.Activated += ArrowKeybindActivated;
            }
            else
            {
                Service.Settings._settingArrowGndBinding.Value.Enabled = false;
                Service.Settings._settingArrowGndBinding.Value.Activated -= ArrowKeybindActivated;
            }
        } catch { }
        
    }
    
    private void ArrowKeybindActivated(object sender, EventArgs e)
    {
        var coords = Service.MapWatch.GetWorldCoordsFromMouseScreenMap();

        var marker = new MarkerCoord();
        marker.FromVector3(coords);
        _markerSet.marks.Add(marker);
        new MarkerEditor(marker, RemoveMarker) { Parent = this };
    }

    private void Debug_CheckedChanged(object sender, CheckChangedEvent e)
    {
        throw new NotImplementedException();
    }

    protected void RemoveMarker(MarkerEditor editor)
    {
        Children.Remove(editor);
        _markerSet.marks.Remove(editor.Marker);
        _AddMarkerButton!.Enabled = _markerSet.marks.Count < 8;
        Invalidate();
    }

 }
