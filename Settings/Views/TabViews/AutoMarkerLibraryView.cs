﻿using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Library.Controls;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class AutoMarkerLibraryView : View
{
    const int HEADER_HEIGHT = 45;
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
            newSet.name = "new set name";
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
            try
            {
                string json = JsonConvert.SerializeObject(_editingMarkerSet);
                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                System.Windows.Forms.Clipboard.SetText(base64);
                ScreenNotification.ShowNotification($"Marker set {_editingMarkerSet?.name} copied to your clipboard!", ScreenNotification.NotificationType.Blue, Service.Textures!._blishHeart, 4);
            } catch (Exception)
            { 
            }
        };
        import.Click += (s, e) =>
        {
            try
            {
                string json = System.Windows.Forms.Clipboard.GetText();

                var bytes = Convert.FromBase64String(json);
                var parsedString = Encoding.UTF8.GetString(bytes);
         
                MarkerSet? markerSet = JsonConvert.DeserializeObject<MarkerSet>(parsedString);
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
                Service.MarkersListing.EditMarker(_editingMarkerSetIndex, _editingMarkerSet!);
            }
            else
            {
                Service.MarkersListing.SaveMarker(_editingMarkerSet!);
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
        GameService.Gw2Mumble.CurrentMap.MapChanged += (s,e) => ReloadMarkerList(_currentMapFilter.Checked);

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

    private bool CanShowPreviewAndPlaceButtons()
    {
        var shouldDoIt =
          Service.Settings.AutoMarker_FeatureEnabled.Value &&
          GameService.GameIntegration.Gw2Instance.Gw2IsRunning &&
          GameService.GameIntegration.Gw2Instance.IsInGame &&
          GameService.Gw2Mumble.IsAvailable;

        if (Service.Settings._settingOnlyWhenCommander.Value || Service.LtMode.Value)
        {
            shouldDoIt &= (GameService.Gw2Mumble.PlayerCharacter.IsCommander || Service.LtMode.Value);
        }
        return shouldDoIt;
    }

    protected void RenderLibraryList(FlowPanel? panel, bool shouldFilter, int currentMapId)
    {
        if (panel == null) return;
        Texture2D editIcon = Service.Textures!._imgArrow;
        Point editSize = new Point(editIcon.Width, editIcon.Height);
        int DetailButtonWidth = panel.Width - ((int)panel.OuterControlPadding.X * 2) - 10;
        var i = 0;
        bool showPlaceBtn = CanShowPreviewAndPlaceButtons();

        panel.Children.Clear();
        _markers.ForEach( marker =>
        {
            var markerIdx = i++;
            var mapName = marker.MapName;

            
            var btn = new DetailsButton()
            {
                Text = (marker.enabled?"":"(Disabled) ")+$"{ marker.name}\n{marker.description}\n{mapName}",
                Icon = marker.enabled?  ((SquadMarker)((i%8))+1).GetIcon(): Service.Textures._imgClear,
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
                Width = marker.MapId == currentMapId && showPlaceBtn ? 210: 340,
            };
            
            if (marker.MapId == currentMapId && showPlaceBtn)
            {
                var Preview = new IconButton()
                {
                    Parent = btn,
                    Icon = Service.Textures!.IconEye,
                    BasicTooltipText = "Preview",
                    Size = new Point(30, 30)
                };
                Preview.MouseEntered += (s, e) => Service.MapWatch.PreviewMarkerSet(marker);
                Preview.MouseLeft += (s, e) => Service.MapWatch.RemovePreviewMarkerSet();
                var placeBtn = new StandardButton()
                {
                    Parent = btn,
                    Icon = Service.Textures!._blishHeartSmall,
                    Text = "Place",
                    Width=100
                };
                placeBtn.Click += (s, e) => Service.MapWatch.PlaceMarkers(marker);
                
            }

            var img = new EnabledIconButton(marker.enabled)
            {
                Size = editSize,
                Parent = btn,
                Opacity = 0.5f
            };
            
            img.Click += (s, e) =>
            {
                marker.enabled = img.WatchValue;
                Service.MarkersListing.EditMarker(markerIdx, marker);
                
            };


            panel.AddFlowControl(btn);
            _markerSetButtons.Add((btn,marker));
        });
    }
}