using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Library.Controls;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Library.Services;
using Manlaan.CommanderMarkers.Settings.Controls;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class AutoMarkerCommunityLibraryView : View
{
    const int HEADER_HEIGHT = 45;
    private CommunityMarkerService _service = new();
    private CommunitySets? _sets;
    private Panel? _listingHeader;
    private FlowPanel? _listingPanel;

    private Dropdown? _categorySelection;
    private Checkbox? _currentMapFilter;


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

        _categorySelection = new Dropdown()
        {
            Parent = _listingHeader,
            Width = 200,
            Location = new(20, 3)
        };
        _categorySelection.Items.Add("Select a category to begin");
        _categorySelection.SelectedItem = _categorySelection.Items[0];
   
        _currentMapFilter = new Checkbox()
        {
            Text = "Filter to current map",
            Parent = _listingHeader,
            Location = new Point(230, 10),
            Checked = Service.Settings.AutoMarker_LibraryFilterToCurrent.Value
        };

        var reload = new NuclearOptionButton()
        {
            Parent = _listingHeader,
            Width = 100,
            Text = "Redownload",
            BasicTooltipText = "Force a redownload of the community library.\n\nHold Ctrl and Shift to activate the button",
            Location = new Point(_listingHeader.Width-100,0)
            
        };
        reload.Click += (s, e) =>
        {
            _sets = _service.FetchListing();
            ReloadMarkerList(_currentMapFilter.Checked);
            ScreenNotification.ShowNotification("Community Library has been reloaded.", ScreenNotification.NotificationType.Info);
        };
        
        _listingPanel = new FlowPanel()
            .BeginFlow(buildPanel, new(-10,-HEADER_HEIGHT), new(0, HEADER_HEIGHT));
        _listingPanel.ControlPadding = new Vector2(0, 10);
        _listingPanel.OuterControlPadding = new Vector2(20, 10);
        _listingPanel.CanScroll= true;

       

        ReloadMarkerList(_currentMapFilter.Checked);
        //Service.MarkersListing.MarkersChanged += (s, e) => ReloadMarkerList(_currentMapFilter.Checked);
        GameService.Gw2Mumble.CurrentMap.MapChanged += (s,e) => ReloadMarkerList(_currentMapFilter.Checked);

        _currentMapFilter.CheckedChanged += (s, e) => {
            Service.Settings.AutoMarker_LibraryFilterToCurrent.Value = _currentMapFilter.Checked;
            ReloadMarkerList(_currentMapFilter.Checked);
        };

        _categorySelection.ValueChanged += (s, e) =>
        {
            ReloadMarkerList(_currentMapFilter.Checked);
        };
    
    }

    protected void ReloadMarkerList(bool filterToCurrent)
    {
        var currentMapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
        if(_sets == null)
        {
            _sets = _service.CommunitySets;
            LoadCatetorySelection();
        }
        RenderLibraryList(_listingPanel,filterToCurrent, currentMapId);
    }
    protected void LoadCatetorySelection()
    {
        _categorySelection?.Items.Clear();
        _sets?.Categories.ForEach(category =>
        {

        _categorySelection?.Items.Add(category?.CategoryName);
        });
    }

    protected void RenderLibraryList(FlowPanel? panel, bool shouldFilter, int currentMapId)
    {
        if (panel == null) return;
        Texture2D editIcon = Service.Textures!.IconImport;
        Point editSize = new Point(editIcon.Width, editIcon.Height);
        int DetailButtonWidth = panel.Width - ((int)panel.OuterControlPadding.X * 2) - 10;
        var i = 0;

        panel.Children.Clear();
        if (_sets == null) return;
        if (_sets.Categories.Count < 1) return;
        var category = _sets.Categories.Find(cat => cat.CategoryName == _categorySelection?.SelectedItem);
        if(category == null) return;
        category.MarkerSets.ForEach( marker =>
        {
            var markerIdx = i++;
            var mapName = marker.MapName;

            
            var btn = new DetailsButton()
            {
                Parent = panel,
                Text = $"{ marker.name}\n{marker.description}",
                Icon = ((SquadMarker)((i%8))+1).GetIcon(),
                Width = DetailButtonWidth,
                IconSize = DetailsIconSize.Small,
                ShowToggleButton = true,
                BasicTooltipText = $"{marker.name}\n{marker.description}\nMap: {mapName}\n\nMarkers in use:\n{marker.DescribeMarkers()}",
                BackgroundColor = marker.enabled? Color.Transparent : new Color(.4f,.1f,.1f,0.1f),
                Visible = shouldFilter ? marker.MapId == currentMapId : true,
            };
            
            new Label()
            {
                Parent = btn,
                Text = $"Author: {marker.Author}",
                Width = marker.MapId==currentMapId ? 180: 300,
                Height=30
            };

            if (marker.MapId == currentMapId)
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
                    Width = 100
                };
                placeBtn.Click += (s, e) => Service.MapWatch.PlaceMarkers(marker);

            }


            var importButton = new StandardButton()
            {
                Icon = Service.Textures!.IconImport,
                Text= "Import",
                BasicTooltipText = "Import this communty marker set into your library",
                Parent = btn,
            };
            
            importButton.Click += (s, e) =>
            {
                Service.MarkersListing.SaveMarker(marker);
                ScreenNotification.ShowNotification($"Imported community marker set into your library", ScreenNotification.NotificationType.Green);
                
            };
        });
    }
}