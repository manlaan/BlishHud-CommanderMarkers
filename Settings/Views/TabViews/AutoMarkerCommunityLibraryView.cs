using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Library.Controls;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class AutoMarkerCommunityLibraryView : View
{
    const int HEADER_HEIGHT = 45;
    private Panel? _listingHeader;
    private FlowPanel? _listingPanel;

    private Dropdown? _categorySelection;
    private Checkbox? _currentMapFilter;
    private Checkbox? _hideImportedFilter;


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
        _categorySelection.Items.Add("All categories");
        _categorySelection.SelectedItem = _categorySelection.Items[0];
   
        _currentMapFilter = new Checkbox()
        {
            Text = "Filter to current map",
            Parent = _listingHeader,
            Location = new Point(230, 10),
            Checked = Service.Settings.AutoMarker_LibraryFilterToCurrent.Value
        };

        _hideImportedFilter = new Checkbox()
        {
            Text = "Only show available",
            Parent = _listingHeader,
            Location = new Point(400, 10),
            Checked = false
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
            Task.Run(() =>
            {
                Service.CommunityCatalog.SyncCatalog();
                GameService.GameThread.Enqueue(() =>
                {
                    LoadCategorySelection();
                    ReloadMarkerList(_currentMapFilter!.Checked);
                    ScreenNotification.ShowNotification("Community Library has been reloaded.", ScreenNotification.NotificationType.Info);
                });
            });
        };
        
        _listingPanel = new FlowPanel()
            .BeginFlow(buildPanel, new(-10,-HEADER_HEIGHT-30), new(0, HEADER_HEIGHT));
        _listingPanel.ControlPadding = new Vector2(0, 10);
        _listingPanel.OuterControlPadding = new Vector2(20, 10);
        _listingPanel.CanScroll= true;

        var contribute = new Label()
        {
            Parent = buildPanel,
            Text = "Submit marker sets from your local library (requires account API permission).",
            AutoSizeWidth= true,
            Location = new Point(10, buildPanel.Height - 28),
        };

        Service.CommunityCatalog.CatalogUpdated += (_, __) =>
        {
            GameService.GameThread.Enqueue(() =>
            {
                LoadCategorySelection();
                ReloadMarkerList(_currentMapFilter!.Checked);
            });
        };

        LoadCategorySelection();
        ReloadMarkerList(_currentMapFilter.Checked);
        GameService.Gw2Mumble.CurrentMap.MapChanged += (s,e) => ReloadMarkerList(_currentMapFilter.Checked);

        _currentMapFilter.CheckedChanged += (s, e) => {
            Service.Settings.AutoMarker_LibraryFilterToCurrent.Value = _currentMapFilter.Checked;
            ReloadMarkerList(_currentMapFilter.Checked);
        };

        _hideImportedFilter.CheckedChanged += (s, e) => ReloadMarkerList(_currentMapFilter.Checked);

        _categorySelection.ValueChanged += (s, e) =>
        {
            ReloadMarkerList(_currentMapFilter.Checked);
        };
    
    }

    protected void ReloadMarkerList(bool filterToCurrent)
    {
        var currentMapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
        RenderLibraryList(_listingPanel, filterToCurrent, currentMapId);
    }

    protected void LoadCategorySelection()
    {
        _categorySelection?.Items.Clear();
        _categorySelection?.Items.Add("All categories");
        foreach (var category in Service.CommunityCatalog.Categories)
        {
            _categorySelection?.Items.Add(category.Name);
        }
        if (_categorySelection != null)
        {
            _categorySelection.SelectedItem = _categorySelection.Items[0];
        }
    }

    private IEnumerable<CommunitySetSummary> VisibleSets(bool filterToCurrent, int currentMapId)
    {
        var selectedCategory = _categorySelection?.SelectedItem;
        foreach (var summary in Service.CommunityCatalog.Sets)
        {
            if (selectedCategory != null && selectedCategory != "All categories" &&
                summary.CategoryName != selectedCategory)
            {
                continue;
            }
            if (filterToCurrent && summary.MapId != currentMapId)
            {
                continue;
            }
            if (_hideImportedFilter?.Checked == true &&
                Service.MarkersListing.ContainsCommunitySetId(summary.Id))
            {
                continue;
            }
            yield return summary;
        }
    }

    protected void RenderLibraryList(FlowPanel? panel, bool shouldFilter, int currentMapId)
    {
        if (panel == null) return;
        int DetailButtonWidth = panel.Width - ((int)panel.OuterControlPadding.X * 2) - 10;
        var i = 0;

        panel.Children.Clear();
        if (Service.CommunityCatalog.Sets.Count < 1)
        {
            panel.AddFlowControl(new Label
            {
                Text = "Community library not loaded yet. Use Redownload (Ctrl+Shift) to fetch.",
                AutoSizeWidth = true
            });
            return;
        }

        foreach (var summary in VisibleSets(shouldFilter, currentMapId))
        {
            var markerIdx = i++;
            var mapName = string.IsNullOrWhiteSpace(summary.MapName)
                ? Service.MapDataCache.Describe(summary.MapId)
                : summary.MapName;

            Service.PreviewImageCache.RequestThumb(summary.Id, summary.PreviewThumbUrl, _ =>
            {
                GameService.GameThread.Enqueue(() => ReloadMarkerList(shouldFilter));
            });

            var thumb = Service.PreviewImageCache.GetThumbTexture(summary.Id, ((SquadMarker)((markerIdx % 8) + 1)).GetIcon());
            var btn = new DetailsButton()
            {
                Parent = panel,
                Text = $"{summary.Name}\n{summary.Description}\n{mapName}",
                Icon = thumb ?? ((SquadMarker)((markerIdx % 8) + 1)).GetIcon(),
                Width = DetailButtonWidth,
                IconSize = DetailsIconSize.Small,
                ShowToggleButton = true,
                BasicTooltipText = $"{summary.Name}\n{summary.Description}\nMap: {mapName}\nAuthor: {summary.Author}",
                BackgroundColor = summary.Enabled ? Color.Transparent : new Color(.4f,.1f,.1f,0.1f),
            };
            
            new Label()
            {
                Parent = btn,
                Text = $"Author: {summary.Author}",
                Width = summary.MapId == currentMapId ? 180: 300,
                Height=30
            };

            if (summary.MapId == currentMapId)
            {
                var Preview = new IconButton()
                {
                    Parent = btn,
                    Icon = Service.Textures!.IconEye,
                    BasicTooltipText = "Preview",
                    Size = new Point(30, 30)
                };
                Preview.MouseEntered += (s, e) =>
                {
                    var markerSet = Service.CommunityCatalog.FetchSetDetail(summary.Id);
                    if (markerSet != null)
                    {
                        Service.MapWatch.PreviewMarkerSet(markerSet);
                    }
                };
                Preview.MouseLeft += (s, e) => Service.MapWatch.RemovePreviewMarkerSet();
                var placeBtn = new StandardButton()
                {
                    Parent = btn,
                    Icon = Service.Textures!._blishHeartSmall,
                    Text = "Place",
                    Width = 100
                };
                placeBtn.Click += (s, e) =>
                {
                    var markerSet = Service.CommunityCatalog.FetchSetDetail(summary.Id);
                    if (markerSet != null)
                    {
                        Service.MapWatch.PlaceMarkers(markerSet);
                    }
                };

            }

            var alreadyImported = Service.MarkersListing.ContainsCommunitySetId(summary.Id);
            var importButton = new StandardButton()
            {
                Icon = Service.Textures!.IconImport,
                Text= alreadyImported ? "Imported" : "Import",
                Enabled = !alreadyImported,
                BasicTooltipText = "Import this community marker set into your library",
                Parent = btn,
            };
            
            importButton.Click += (s, e) =>
            {
                var markerSet = Service.CommunityCatalog.FetchSetDetail(summary.Id);
                if (markerSet == null)
                {
                    ScreenNotification.ShowNotification("Failed to fetch marker set from server.", ScreenNotification.NotificationType.Error);
                    return;
                }
                Service.MarkersListing.SaveMarker(markerSet);
                ScreenNotification.ShowNotification($"Imported \"{summary.Name}\" into your library", ScreenNotification.NotificationType.Green);
                ReloadMarkerList(shouldFilter);
            };

            panel.AddFlowControl(btn);
        }
    }
}
