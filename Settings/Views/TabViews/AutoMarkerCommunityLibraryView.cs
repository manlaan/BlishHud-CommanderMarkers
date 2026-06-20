using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Gw2Sharp.WebApi.V2.Models;
using Manlaan.CommanderMarkers.Library.Controls;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Library.Services;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Presets.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class AutoMarkerCommunityLibraryView : View
{
    const int HEADER_HEIGHT = 45;
    const int SHARE_SECTION_HEIGHT = 190;

    private Panel? _listingHeader;
    private FlowPanel? _listingPanel;
    private FlowPanel? _sharePanel;

    private Dropdown? _categorySelection;
    private Checkbox? _currentMapFilter;
    private Checkbox? _hideImportedFilter;
    private TextBox? _searchBox;

    private readonly Dictionary<string, ShareRowState> _shareRows = new();

    private sealed class ShareRowState
    {
        public int CategoryIndex { get; set; }
        public string CustomCategory { get; set; } = "";
        public string Error { get; set; } = "";
        public string Status { get; set; } = "";
        public bool Sharing { get; set; }
    }

    protected override void Build(Container buildPanel)
    {
        base.Build(buildPanel);

        _listingHeader = new Panel()
        {
            Parent = buildPanel,
            Size = new Point(buildPanel.Width, HEADER_HEIGHT),
            Location = new Point(0, 0),
            ShowBorder = true,
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

        _searchBox = new TextBox()
        {
            Parent = _listingHeader,
            Width = LibrarySearch.SearchFieldWidth,
            Location = new Point(_listingHeader.Width - LibrarySearch.SearchFieldWidth - 120, 8),
            BasicTooltipText = "Search name, description, author, or map"
        };
        _searchBox.TextChanged += (_, __) => ReloadMarkerList(_currentMapFilter!.Checked);

        var reload = new NuclearOptionButton()
        {
            Parent = _listingHeader,
            Width = 100,
            Text = "Redownload",
            BasicTooltipText = "Force a redownload of the community library.\n\nHold Ctrl and Shift to activate the button",
            Location = new Point(_listingHeader.Width - 100, 0)
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
                    RenderShareSection();
                    ScreenNotification.ShowNotification("Community Library has been reloaded.",
                        ScreenNotification.NotificationType.Info);
                });
            });
        };

        _listingPanel = new FlowPanel()
            .BeginFlow(buildPanel, new(-10, -HEADER_HEIGHT - SHARE_SECTION_HEIGHT - 10), new(0, HEADER_HEIGHT));
        _listingPanel.ControlPadding = new Vector2(0, 10);
        _listingPanel.OuterControlPadding = new Vector2(20, 10);
        _listingPanel.CanScroll = true;

        _sharePanel = new FlowPanel()
        {
            Parent = buildPanel,
            Location = new Point(0, buildPanel.Height - SHARE_SECTION_HEIGHT),
            Size = new Point(buildPanel.Width, SHARE_SECTION_HEIGHT),
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(20, 8),
            ControlPadding = new Vector2(0, 6),
            CanScroll = true,
            ShowBorder = true
        };

        Service.CommunityCatalog.CatalogUpdated += (_, __) =>
        {
            GameService.GameThread.Enqueue(() =>
            {
                LoadCategorySelection();
                ReloadMarkerList(_currentMapFilter!.Checked);
                RenderShareSection();
            });
        };

        LoadCategorySelection();
        ReloadMarkerList(_currentMapFilter.Checked);
        RenderShareSection();
        GameService.Gw2Mumble.CurrentMap.MapChanged += (s, e) => ReloadMarkerList(_currentMapFilter!.Checked);

        _currentMapFilter.CheckedChanged += (s, e) =>
        {
            Service.Settings.AutoMarker_LibraryFilterToCurrent.Value = _currentMapFilter.Checked;
            ReloadMarkerList(_currentMapFilter.Checked);
        };

        _hideImportedFilter.CheckedChanged += (s, e) => ReloadMarkerList(_currentMapFilter!.Checked);

        _categorySelection.ValueChanged += (s, e) => ReloadMarkerList(_currentMapFilter!.Checked);
    }

    protected void ReloadMarkerList(bool filterToCurrent)
    {
        var currentMapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
        RenderLibraryList(_listingPanel, filterToCurrent, currentMapId);
    }

    protected void LoadCategorySelection()
    {
        var selected = _categorySelection?.SelectedItem as string;
        _categorySelection?.Items.Clear();
        _categorySelection?.Items.Add("All categories");

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in Service.CommunityCatalog.Categories)
        {
            if (!string.IsNullOrWhiteSpace(category.Name) && seen.Add(category.Name))
            {
                _categorySelection?.Items.Add(category.Name);
            }
        }

        if (_categorySelection!.Items.Count <= 1)
        {
            foreach (var summary in Service.CommunityCatalog.Sets)
            {
                if (!string.IsNullOrWhiteSpace(summary.CategoryName) && seen.Add(summary.CategoryName))
                {
                    _categorySelection.Items.Add(summary.CategoryName);
                }
            }
        }

        if (selected != null && _categorySelection.Items.Contains(selected))
        {
            _categorySelection.SelectedItem = selected;
        }
        else
        {
            _categorySelection.SelectedItem = _categorySelection.Items[0];
        }
    }

    private IEnumerable<CommunitySetSummary> VisibleSets(bool filterToCurrent, int currentMapId)
    {
        var selectedCategory = _categorySelection?.SelectedItem as string;
        var searchLower = LibrarySearch.ToLowerCopy(_searchBox?.Text ?? "");
        foreach (var summary in Service.CommunityCatalog.Sets)
        {
            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "All categories" &&
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

            var mapName = string.IsNullOrWhiteSpace(summary.MapName)
                ? Service.MapDataCache.Describe(summary.MapId)
                : summary.MapName;
            if (!LibrarySearch.MatchesCommunity(summary, mapName, searchLower))
            {
                continue;
            }

            yield return summary;
        }
    }

    protected void RenderLibraryList(FlowPanel? panel, bool shouldFilter, int currentMapId)
    {
        if (panel == null) return;
        int detailButtonWidth = panel.Width - ((int)panel.OuterControlPadding.X * 2) - 10;
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

            var thumb = Service.PreviewImageCache.GetThumbTexture(summary.Id,
                ((SquadMarker)((markerIdx % 8) + 1)).GetIcon());
            var btn = new DetailsButton()
            {
                Parent = panel,
                Text = $"{summary.Name}\n{summary.Description}\n{mapName}",
                Icon = thumb ?? ((SquadMarker)((markerIdx % 8) + 1)).GetIcon(),
                Width = detailButtonWidth,
                IconSize = DetailsIconSize.Small,
                ShowToggleButton = true,
                BasicTooltipText = $"{summary.Name}\n{summary.Description}\nMap: {mapName}\nAuthor: {summary.Author}",
                BackgroundColor = summary.Enabled ? Color.Transparent : new Color(.4f, .1f, .1f, 0.1f),
            };

            if (thumb != null)
            {
                MapPreviewTooltipService.Attach(btn, MapPreviewTarget.FromCommunitySummary(summary));
            }

            new Label()
            {
                Parent = btn,
                Text = $"Author: {summary.Author}",
                Width = summary.MapId == currentMapId ? 180 : 300,
                Height = 30
            };

            if (summary.MapId == currentMapId)
            {
                var preview = new IconButton()
                {
                    Parent = btn,
                    Icon = Service.Textures!.IconEye,
                    BasicTooltipText = "Hover to preview markers on the map",
                    Size = new Point(30, 30)
                };
                preview.MouseEntered += (s, e) =>
                {
                    var markerSet = Service.CommunityCatalog.FetchSetDetail(summary.Id);
                    if (markerSet != null)
                    {
                        Service.MapWatch.PreviewMarkerSet(markerSet);
                    }
                };
                preview.MouseLeft += (s, e) => Service.MapWatch.RemovePreviewMarkerSet();
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
                Text = alreadyImported ? "Imported" : "Import",
                Enabled = !alreadyImported,
                BasicTooltipText = "Import this community marker set into your library",
                Parent = btn,
            };

            importButton.Click += (s, e) =>
            {
                var markerSet = Service.CommunityCatalog.FetchSetDetail(summary.Id);
                if (markerSet == null)
                {
                    ScreenNotification.ShowNotification("Failed to fetch marker set from server.",
                        ScreenNotification.NotificationType.Error);
                    return;
                }
                Service.MarkersListing.SaveMarker(markerSet);
                ScreenNotification.ShowNotification($"Imported \"{summary.Name}\" into your library",
                    ScreenNotification.NotificationType.Green);
                ReloadMarkerList(shouldFilter);
                RenderShareSection();
            };

            panel.AddFlowControl(btn);
        }
    }

    private List<string> ShareCategoryNames()
    {
        var names = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in Service.CommunityCatalog.Categories)
        {
            if (!string.IsNullOrWhiteSpace(category.Name) && seen.Add(category.Name))
            {
                names.Add(category.Name);
            }
        }
        if (names.Count == 0)
        {
            foreach (var summary in Service.CommunityCatalog.Sets)
            {
                if (!string.IsNullOrWhiteSpace(summary.CategoryName) && seen.Add(summary.CategoryName))
                {
                    names.Add(summary.CategoryName);
                }
            }
        }
        return names;
    }

    private string ResolveShareCategory(ShareRowState rowState, IReadOnlyList<string> categoryNames)
    {
        var customIndex = categoryNames.Count;
        if (rowState.CategoryIndex == customIndex || categoryNames.Count == 0)
        {
            return rowState.CustomCategory.Trim();
        }
        if (rowState.CategoryIndex >= 0 && rowState.CategoryIndex < categoryNames.Count)
        {
            return categoryNames[rowState.CategoryIndex];
        }
        return rowState.CustomCategory.Trim();
    }

    private void RenderShareSection()
    {
        if (_sharePanel == null)
        {
            return;
        }

        _sharePanel.Children.Clear();

        _ = new Label
        {
            Parent = _sharePanel,
            Text = "Share with community",
            AutoSizeWidth = true
        };

        if (Service.Gw2ApiManager == null || !Service.Gw2ApiManager.HasPermission(TokenPermission.Account))
        {
            _ = new Label
            {
                Parent = _sharePanel,
                Text = "Enable the Account API permission for Commander Markers in BlishHUD to share marker sets.",
                Width = _sharePanel.Width - 40,
                WrapText = true
            };
            return;
        }

        var categoryNames = ShareCategoryNames();
        var shareable = Service.MarkersListing.GetAllMarkerSets()
            .Where(MarkerListing.IsShareableWithCommunity)
            .ToList();

        if (shareable.Count == 0)
        {
            _ = new Label
            {
                Parent = _sharePanel,
                Text = "No custom marker sets available to share.",
                AutoSizeWidth = true
            };
            return;
        }

        foreach (var markerSet in shareable)
        {
            var setId = markerSet.id ?? Guid.NewGuid().ToString();
            if (!_shareRows.TryGetValue(setId, out var rowState))
            {
                rowState = new ShareRowState();
                _shareRows[setId] = rowState;
            }

            var row = new Panel
            {
                Parent = _sharePanel,
                Width = _sharePanel.Width - 40,
                Height = rowState.CategoryIndex == categoryNames.Count || categoryNames.Count == 0 ? 72 : 48
            };

            _ = new Label
            {
                Parent = row,
                Text = markerSet.name,
                Location = new Point(0, 0),
                Width = 260,
                AutoSizeHeight = true
            };

            var categoryDropdown = new Dropdown
            {
                Parent = row,
                Location = new Point(270, 0),
                Width = 180
            };
            foreach (var name in categoryNames)
            {
                categoryDropdown.Items.Add(name);
            }
            categoryDropdown.Items.Add("Custom...");
            var customIndex = categoryNames.Count;
            if (rowState.CategoryIndex < 0 || rowState.CategoryIndex > customIndex)
            {
                rowState.CategoryIndex = categoryNames.Count == 0 ? customIndex : 0;
            }
            if (rowState.CategoryIndex < categoryDropdown.Items.Count)
            {
                categoryDropdown.SelectedItem = categoryDropdown.Items[rowState.CategoryIndex];
            }
            categoryDropdown.ValueChanged += (_, __) =>
            {
                rowState.CategoryIndex = categoryDropdown.Items.IndexOf(categoryDropdown.SelectedItem);
            };

            TextBox? customCategoryBox = null;
            if (rowState.CategoryIndex == customIndex || categoryNames.Count == 0)
            {
                customCategoryBox = new TextBox
                {
                    Parent = row,
                    Location = new Point(270, 28),
                    Width = 180,
                    Text = rowState.CustomCategory,
                    BasicTooltipText = "Type category name"
                };
                customCategoryBox.TextChanged += (_, __) => rowState.CustomCategory = customCategoryBox.Text;
            }

            var shareButton = new StandardButton
            {
                Parent = row,
                Location = new Point(460, 0),
                Width = 160,
                Text = rowState.Sharing ? "Sharing..." : "Share with community",
                Icon = Service.Textures!.IconImport,
                Enabled = !rowState.Sharing
            };

            if (!string.IsNullOrWhiteSpace(rowState.Error))
            {
                _ = new Label
                {
                    Parent = row,
                    Text = rowState.Error,
                    Location = new Point(460, 28),
                    Width = 220
                };
            }
            else if (!string.IsNullOrWhiteSpace(rowState.Status))
            {
                _ = new Label
                {
                    Parent = row,
                    Text = rowState.Status,
                    Location = new Point(460, 28),
                    Width = 220
                };
            }

            shareButton.Click += (_, __) =>
            {
                if (customCategoryBox != null)
                {
                    rowState.CustomCategory = customCategoryBox.Text;
                }

                var category = ResolveShareCategory(rowState, categoryNames);
                if (string.IsNullOrWhiteSpace(category))
                {
                    rowState.Error = "Enter a category name.";
                    rowState.Status = "";
                    RenderShareSection();
                    return;
                }

                rowState.Sharing = true;
                rowState.Error = "";
                rowState.Status = "";
                RenderShareSection();

                var capturedSet = markerSet;
                var capturedCategory = category;
                var capturedRow = rowState;
                Task.Run(async () =>
                {
                    try
                    {
                        var subtoken = await Service.SubtokenService.GetValidSubtokenAsync().ConfigureAwait(false);
                        if (string.IsNullOrEmpty(subtoken))
                        {
                            capturedRow.Error = "Account API permission required.";
                            return;
                        }

                        var payload = MarkerSetSubmission.ToSubmissionPayload(capturedSet, capturedCategory);
                        using var client = new WebClient();
                        client.Headers[HttpRequestHeader.Authorization] = "Bearer " + subtoken;
                        client.Headers[HttpRequestHeader.ContentType] = "application/json";
                        var url = Service.ManifestService.Manifest.Absolute(Service.ManifestService.Manifest.SubmissionsUrl);
                        client.UploadString(url, payload.ToString(Newtonsoft.Json.Formatting.None));
                        capturedRow.Status = "Sent for moderator review.";
                    }
                    catch (WebException ex) when (ex.Response is HttpWebResponse response)
                    {
                        capturedRow.Error = $"Share failed ({(int)response.StatusCode}).";
                    }
                    catch (Exception)
                    {
                        capturedRow.Error = "Share failed.";
                    }
                    finally
                    {
                        capturedRow.Sharing = false;
                        GameService.GameThread.Enqueue(RenderShareSection);
                    }
                });
            };
        }
    }
}
