using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Library.Controls;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Settings.Controls;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class AutoMarkerLibraryView : View
{
    const int HEADER_HEIGHT = 40;
    private List<MarkerSet> _markers = new();
    private Panel? _listingHeader;
    private Panel? _detailsHeader;
    private FlowPanel? _listingPanel;
    private MarkerSetEditor? _detailsPanel;
    private bool _showingDetails = false;
    private List<(DetailsButton,MarkerSet)> _markerSetButtons = new List<(DetailsButton, MarkerSet)>();

    private Checkbox? _currentMapFilter;


    private MarkerSet? _editingMarkerSet;
    private int _editingMarkerSetIndex=-1;
    protected override void Build(Container buildPanel)
    {
        base.Build(buildPanel);
        
        _listingHeader = new Panel()
        {
            Parent = buildPanel,
            Size = new Point(buildPanel.Width, HEADER_HEIGHT),
            Location = new Point(0, 0),
            ShowBorder= true,
        };
        _detailsHeader = new Panel()
        {
            Parent = buildPanel,
            Size = new Point(buildPanel.Width, HEADER_HEIGHT),
            Location = new Point(0, buildPanel.Height - HEADER_HEIGHT),
            ShowBorder = true,
            Visible = false,
            ClipsBounds = false
        };
        
        var newMarkerSet = new StandardButton()
        {
            Text = "Add New",
            Parent = _listingHeader,
            Width = 200,
            Location = new Point(20, 3)
        };
        _currentMapFilter = new Checkbox()
        {
            Text = "Filter to current map",
            Parent = _listingHeader,
            Location = new Point(230, 10),
            Checked = Service.Settings.AutoMarker_LibraryFilterToCurrent.Value
        };
        newMarkerSet.Click += (s, e) =>
        {
            var newSet = new MarkerSet();
            newSet.name = "new marker set name";
            newSet.description = "description";
            newSet.mapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
            newSet.trigger = new WorldCoord();
            //newSet.trigger.SetFromMumbleLocation();
            var mark = new MarkerCoord();
            mark.name = "marker name";
            newSet.marks.Add(mark);
            SwapView(newSet, -1);
        };

        var cancelButton = new StandardButton()
        {
            Parent = _detailsHeader,
            Text = "Cancel",
            Width= 100,
            Location = new Point(10, 0),
            Icon = Service.Textures!.IconGoBack
        };
        var saveButton = new StandardButton()
        {
            Parent = _detailsHeader,
            Text = "Save",
            Width = 100,
            Location = new Point(115, 0),
            Icon = Service.Textures!.IconSave
        };
        var export = new StandardButton()
        {
            Parent = _detailsHeader,
            Text = "Export",
            Width = 95,
            Icon = Service.Textures!.IconExport,
            Location = new Point(220, 0),
            BasicTooltipText = "Export this marker set to your clipboard to share with others"
        };
        var import = new StandardButton()
        {
            Parent = _detailsHeader,
            Text = "Import",
            Width = 95,
            Location = new Point(320, 0),
            Icon =Service.Textures!.IconImport,
            BasicTooltipText = "Copy a marker set to your clipboard, then import it by clicking this button"
        };
        var deleteButton = new StandardButton()
        {
            Parent = _detailsHeader,
            Icon = Service.Textures!.IconDelete,
            Width = 80,
            Text = "Delete",
            BasicTooltipText = $"Delete Marker Set",
            Location = new Point(420, 0)
        };

        cancelButton.Click += (s, e) => SwapView(false);
        export.Click += (s, e) =>
        {
            string json = JsonConvert.SerializeObject(_editingMarkerSet);
            Debug.WriteLine(json);
            System.Windows.Forms.Clipboard.SetText(json);
            ScreenNotification.ShowNotification($"Marker set {_editingMarkerSet?.name} copied to your clipboard!", ScreenNotification.NotificationType.Blue, Service.Textures!._blishHeart, 4);
        };
        import.Click += (s, e) =>
        {
            try
            {
                string json = System.Windows.Forms.Clipboard.GetText();
                MarkerSet? markerSet = JsonConvert.DeserializeObject<MarkerSet>(json);
                if (markerSet == null)
                {
                    throw new Exception("Invalid JSON");
                }
                ScreenNotification.ShowNotification($"Imported marker set {markerSet.name}", ScreenNotification.NotificationType.Green, Service.Textures!._blishHeart, 4);
                _editingMarkerSet?.CloneFromMarkerSet(markerSet);
                _detailsPanel!.LoadMarkerSet(_editingMarkerSet, _editingMarkerSetIndex);

            }
            catch (Exception)
            {
                ScreenNotification.ShowNotification("Unable to import clipboard content\nDid you copy a marker set first?", ScreenNotification.NotificationType.Red, null, 5);
            }

        };   
        saveButton.Click += (s, e) =>
        {
            if (_editingMarkerSetIndex >= 0)
            {
                Debug.WriteLine($"existing{_editingMarkerSet.marks[0].icon}");
                Service.MarkersListing.EditMarker(_editingMarkerSetIndex, _editingMarkerSet!);
            }
            else
            {
                Debug.WriteLine($"new set{_editingMarkerSet.marks[0].icon}");
                Service.MarkersListing.SaveMarker(_editingMarkerSet!);
            }
            SwapView(true);
        };
        deleteButton.Click += (s, e) =>
        {
            if (_editingMarkerSetIndex > 0)
            {
                Service.MarkersListing.DeleteMarker(_editingMarkerSet!);
            }
            SwapView(true);
        };

        _listingPanel = new FlowPanel()
            .BeginFlow(buildPanel, new(-10,-HEADER_HEIGHT), new(0, HEADER_HEIGHT));
        _listingPanel.ControlPadding = new Vector2(0, 10);
        _listingPanel.OuterControlPadding = new Vector2(20, 10);
        _listingPanel.CanScroll= true;
        _listingPanel.Visible = !_showingDetails;

        _detailsPanel = new MarkerSetEditor(SwapView)
        {
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(20, 10),
            Parent = buildPanel,
            Size = buildPanel.Size + new Point(-10,-HEADER_HEIGHT),
            ShowBorder = true,
            Location = new(0, 0),
        };
        _detailsPanel.Visible = _showingDetails;
        _detailsPanel.CanScroll = true;


        ReloadMarkerList(_currentMapFilter.Checked);
        Service.MarkersListing.MarkersChanged += (s, e) => ReloadMarkerList(_currentMapFilter.Checked);

        _currentMapFilter.CheckedChanged += (s, e) => {
            Service.Settings.AutoMarker_LibraryFilterToCurrent.Value = _currentMapFilter.Checked;
            ReloadMarkerList(_currentMapFilter.Checked);
        };
    
    }



    protected void SwapView(bool wasUpdated)
    {
        if (!wasUpdated)
        {
            Service.MarkersListing.ReloadFromFile();
        }

        _showingDetails = false;
        _listingHeader!.Visible = !_showingDetails;
        _detailsHeader!.Visible = _showingDetails;
        _detailsPanel!.Visible = _showingDetails;

        var currentScroll = _listingPanel!.VerticalScrollOffset;
        ReloadMarkerList(_currentMapFilter!.Checked);
        _listingPanel.VerticalScrollOffset = currentScroll;

        _listingPanel!.Visible = !_showingDetails;
    }
    protected void SwapView(MarkerSet marker, int idx) 
    {
        _editingMarkerSet = marker; 
        _editingMarkerSetIndex = idx;
        _showingDetails = true;
        _listingHeader!.Visible = !_showingDetails;
        _listingPanel!.Visible = !_showingDetails;
        _detailsHeader!.Visible = _showingDetails;
        _detailsPanel!.Visible = _showingDetails;
        _detailsPanel!.LoadMarkerSet(marker, idx);
        
    }

    protected void ReloadMarkerList(bool filterToCurrent)
    {
        var currentMapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
        _markers = Service.MarkersListing.GetAllMarkerSets();
        RenderLibraryList(_listingPanel,filterToCurrent, currentMapId);
    }

    protected void RenderLibraryList(FlowPanel? panel, bool shouldFilter, int currentMapId)
    {
        if (panel == null) return;
        Texture2D editIcon = Service.Textures!._imgArrow;
        Point editSize = new Point(editIcon.Width, editIcon.Height);
        int DetailButtonWidth = panel.Width - ((int)panel.OuterControlPadding.X * 2) - 10;
        var i = 0;

        panel.Children.Clear();

        _markers.ForEach( marker =>
        {
            var markerIdx = i++;
            var mapName = marker.MapName;

            
            var btn = new DetailsButton()
            {
                Text = (marker.enabled?"":"(Disabled) ")+$"{ marker.name}\n{marker.description}",
                Icon = marker.enabled? Service.Textures._imgHeart : Service.Textures._imgClear,
                Width = DetailButtonWidth,
                IconSize = DetailsIconSize.Small,
                ShowToggleButton = true,
                BasicTooltipText = $"{marker.name}\n{marker.description}\nMap: {mapName}\n\nMarkers in use:\n{marker.DescribeMarkers()}",
                BackgroundColor = marker.enabled? Color.Transparent : new Color(.4f,.1f,.1f,0.1f),
                Visible = shouldFilter ? marker.MapId == currentMapId : true,
            };
            var edit = new StandardButton()
            {
                Parent = btn,
                Text = "Edit",
                Width=60,
                BasicTooltipText = $"Click to edit {marker.name}",
                Icon = Service.Textures!.IconEdit
            };
            edit.Click += (s, e) => {
                SwapView(marker, markerIdx);
            };
            new Label()
            {
                Parent = btn,
                Text = mapName,
                Width = 300,
                BasicTooltipText = mapName,
                Height=30
            };

            var img = new EnabledIconButton(marker.enabled)
            {
                Size = editSize,
                Parent = btn,
            };
            
            img.Click += (s, e) =>
            {
                marker.enabled = img.WatchValue;
                Debug.WriteLine($"marker img click- set={marker.name}, index ={markerIdx}");
                Service.MarkersListing.EditMarker(markerIdx, marker);
                //SwapView();
                
            };


            panel.AddFlowControl(btn);
            _markerSetButtons.Add((btn,marker));
        });
    }
}