using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Library.Controls;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Presets.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TokenPermission = Gw2Sharp.WebApi.V2.Models.TokenPermission;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class AutoMarkerLibraryView : View
{
    const int HEADER_HEIGHT = 45;

    private enum LibraryViewMode
    {
        List,
        Edit,
        Share
    }

    private List<MarkerSet> _markers = new();
    private Panel? _listingHeader;
    private Panel? _detailsHeader;
    private Panel? _shareHeader;
    private FlowPanel? _listingPanel;
    private MarkerSetEditor? _detailsPanel;
    private MarkerSetSharePanel? _sharePanel;
    private LibraryViewMode _viewMode = LibraryViewMode.List;

    private Checkbox? _currentMapFilter;
    private Checkbox? _mineFilter;
    private TextBox? _searchBox;
    private StandardButton? _shareSubmitButton;

    private MarkerSet? _editingMarkerSet;
    private MarkerSet? _sharingMarkerSet;
    private int _editingMarkerSetIndex = -1;

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
        _detailsHeader = new Panel()
        {
            Parent = buildPanel,
            Size = new Point(buildPanel.Width, HEADER_HEIGHT),
            Location = new Point(0, buildPanel.Height - HEADER_HEIGHT),
            ShowBorder = true,
            Visible = false,
            ClipsBounds = false
        };
        _shareHeader = new Panel()
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
            Width = 95,
            Location = new Point(20, 3)
        };
        _currentMapFilter = new Checkbox()
        {
            Text = "Current map",
            Parent = _listingHeader,
            Location = new Point(newMarkerSet.Right + 5, 10),
            Checked = Service.Settings.AutoMarker_LibraryFilterToCurrent.Value,
            BasicTooltipText = "Only show marker sets for your current map"
        };
        _mineFilter = new Checkbox()
        {
            Text = "Mine",
            Parent = _listingHeader,
            Location = new Point(_currentMapFilter.Right + 8, 10),
            Checked = Service.Settings.AutoMarker_LibraryFilterMine.Value,
            BasicTooltipText = "Hide marker sets imported from the community library"
        };
        _searchBox = new TextBox()
        {
            Parent = _listingHeader,
            Width = LibrarySearch.SearchFieldWidth,
            Location = new Point(_listingHeader.Width - LibrarySearch.SearchFieldWidth - 20, 8),
            BasicTooltipText = "Search name, description, or map"
        };
        _searchBox.TextChanged += (_, __) => ReloadMarkerList(_currentMapFilter!.Checked);
        newMarkerSet.Click += (s, e) =>
        {
            var newSet = new MarkerSet();
            newSet.id = Guid.NewGuid().ToString();
            newSet.source = "custom";
            newSet.name = "new set name";
            newSet.description = "description";
            newSet.mapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
            newSet.trigger = new WorldCoord();
            var mark = new MarkerCoord();
            mark.name = "marker name";
            newSet.marks.Add(mark);
            SwapView(newSet, -1);
        };

        var cancelButton = new StandardButton()
        {
            Parent = _detailsHeader,
            Text = "Cancel",
            Width = 100,
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
            Icon = Service.Textures!.IconImport,
            BasicTooltipText = "Copy a marker set to your clipboard, then import it by clicking this button"
        };
        var deleteButton = new StandardButton()
        {
            Parent = _detailsHeader,
            Icon = Service.Textures!.IconDelete,
            Width = 80,
            Text = "Delete",
            BasicTooltipText = "Delete Marker Set",
            Location = new Point(420, 0)
        };

        cancelButton.Click += (s, e) => SwapView(false);
        export.Click += (s, e) =>
        {
            try
            {
                if (_editingMarkerSet == null)
                {
                    return;
                }
                var base64 = MarkerSetShareCode.Export(_editingMarkerSet);
                System.Windows.Forms.Clipboard.SetText(base64);
                ScreenNotification.ShowNotification($"Marker set {_editingMarkerSet.name} copied to your clipboard!",
                    ScreenNotification.NotificationType.Blue, Service.Textures!._blishHeart, 4);
            }
            catch (Exception)
            {
            }
        };
        import.Click += (s, e) =>
        {
            try
            {
                string json = System.Windows.Forms.Clipboard.GetText();
                MarkerSet? markerSet = MarkerSetShareCode.Import(json, (communitySetId, name) =>
                    Service.CommunityCatalog.FetchSetDetail(communitySetId));
                if (markerSet == null)
                {
                    throw new Exception("Invalid share code");
                }
                ScreenNotification.ShowNotification($"Imported marker set {markerSet.name}",
                    ScreenNotification.NotificationType.Green, Service.Textures!._blishHeart, 4);
                _editingMarkerSet?.CloneFromMarkerSet(markerSet);
                _detailsPanel!.LoadMarkerSet(_editingMarkerSet, _editingMarkerSetIndex);
            }
            catch (Exception)
            {
                ScreenNotification.ShowNotification("Unable to import clipboard content\nDid you copy a marker set first?",
                    ScreenNotification.NotificationType.Red, null, 5);
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
                if (string.IsNullOrWhiteSpace(_editingMarkerSet!.id))
                {
                    _editingMarkerSet.id = Guid.NewGuid().ToString();
                }
                if (string.IsNullOrWhiteSpace(_editingMarkerSet.source))
                {
                    _editingMarkerSet.source = "custom";
                }
                Service.MarkersListing.SaveMarker(_editingMarkerSet!);
            }
            SwapView(true);
        };
        deleteButton.Click += (s, e) =>
        {
            if (_editingMarkerSetIndex >= 0)
            {
                Service.MarkersListing.DeleteMarker(_editingMarkerSet!);
            }
            SwapView(true);
        };

        var shareCancelButton = new StandardButton()
        {
            Parent = _shareHeader,
            Text = "Cancel",
            Width = 100,
            Location = new Point(10, 0),
            Icon = Service.Textures!.IconGoBack
        };
        _shareSubmitButton = new StandardButton()
        {
            Parent = _shareHeader,
            Text = "Submit",
            Width = 110,
            Location = new Point(115, 0),
            Icon = Service.Textures!.IconImport,
            BasicTooltipText = "Send this marker set for community moderator review"
        };
        shareCancelButton.Click += (_, __) => CloseShareView();
        _shareSubmitButton.Click += (_, __) => SubmitShare();

        _listingPanel = new FlowPanel()
            .BeginFlow(buildPanel, new(-10, -HEADER_HEIGHT), new(0, HEADER_HEIGHT));
        _listingPanel.ControlPadding = new Vector2(0, 10);
        _listingPanel.OuterControlPadding = new Vector2(20, 10);
        _listingPanel.CanScroll = true;
        _listingPanel.Visible = _viewMode == LibraryViewMode.List;

        _detailsPanel = new MarkerSetEditor(SwapView)
        {
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(20, 10),
            Parent = buildPanel,
            Size = buildPanel.Size + new Point(-10, -HEADER_HEIGHT),
            ShowBorder = true,
            Location = new(0, 0),
        };
        _detailsPanel.Visible = _viewMode == LibraryViewMode.Edit;
        _detailsPanel.CanScroll = true;

        _sharePanel = new MarkerSetSharePanel
        {
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            OuterControlPadding = new Vector2(20, 10),
            Parent = buildPanel,
            Size = buildPanel.Size + new Point(-10, -HEADER_HEIGHT),
            ShowBorder = true,
            Location = new(0, 0),
        };
        _sharePanel.Visible = _viewMode == LibraryViewMode.Share;
        _sharePanel.CanScroll = true;

        ApplyViewMode();

        ReloadMarkerList(_currentMapFilter.Checked);
        Service.MarkersListing.MarkersChanged += (s, e) => ReloadMarkerList(_currentMapFilter!.Checked);
        GameService.Gw2Mumble.CurrentMap.MapChanged += (s, e) => ReloadMarkerList(_currentMapFilter!.Checked);

        _currentMapFilter.CheckedChanged += (s, e) =>
        {
            Service.Settings.AutoMarker_LibraryFilterToCurrent.Value = _currentMapFilter.Checked;
            ReloadMarkerList(_currentMapFilter.Checked);
        };
        _mineFilter.CheckedChanged += (_, __) =>
        {
            Service.Settings.AutoMarker_LibraryFilterMine.Value = _mineFilter.Checked;
            ReloadMarkerList(_currentMapFilter!.Checked);
        };
    }

    protected void SwapView(bool wasUpdated)
    {
        if (!wasUpdated)
        {
            Service.MarkersListing.ReloadFromFile();
        }

        _viewMode = LibraryViewMode.List;
        _editingMarkerSet = null;
        _sharingMarkerSet = null;
        ApplyViewMode();

        var currentScroll = _listingPanel!.VerticalScrollOffset;
        ReloadMarkerList(_currentMapFilter!.Checked);
        _listingPanel.VerticalScrollOffset = currentScroll;
    }

    protected void SwapView(MarkerSet marker, int idx)
    {
        if (MarkerListing.IsCommunityLinked(marker))
        {
            OpenPersonalizedEditor(marker);
            return;
        }

        _editingMarkerSet = marker;
        _editingMarkerSetIndex = idx;
        _sharingMarkerSet = null;
        _viewMode = LibraryViewMode.Edit;
        ApplyViewMode();
        _detailsPanel!.LoadMarkerSet(marker, idx);
    }

    private void OpenPersonalizedEditor(MarkerSet template)
    {
        var personalized = MarkerListing.DuplicateAsEditableCopy(template);
        _editingMarkerSet = personalized;
        _editingMarkerSetIndex = -1;
        _sharingMarkerSet = null;
        _viewMode = LibraryViewMode.Edit;
        ApplyViewMode();
        _detailsPanel!.LoadMarkerSet(personalized, -1);
    }

    private void OpenShareView(MarkerSet marker)
    {
        _sharingMarkerSet = marker;
        _editingMarkerSet = null;
        _editingMarkerSetIndex = -1;
        _viewMode = LibraryViewMode.Share;
        ApplyViewMode();
        _sharePanel!.LoadMarkerSet(marker);
    }

    private void CloseShareView()
    {
        _sharingMarkerSet = null;
        _viewMode = LibraryViewMode.List;
        ApplyViewMode();

        var currentScroll = _listingPanel!.VerticalScrollOffset;
        ReloadMarkerList(_currentMapFilter!.Checked);
        _listingPanel.VerticalScrollOffset = currentScroll;
    }

    private void ApplyViewMode()
    {
        var showList = _viewMode == LibraryViewMode.List;
        var showEdit = _viewMode == LibraryViewMode.Edit;
        var showShare = _viewMode == LibraryViewMode.Share;

        _listingHeader!.Visible = showList;
        _listingPanel!.Visible = showList;
        _detailsHeader!.Visible = showEdit;
        _detailsPanel!.Visible = showEdit;
        _shareHeader!.Visible = showShare;
        _sharePanel!.Visible = showShare;

        if (_shareSubmitButton != null)
        {
            var canSubmit = showShare && CanShareMarkerSets() && !(_sharePanel?.IsSubmitting ?? false);
            _shareSubmitButton.Enabled = canSubmit;
            _shareSubmitButton.Text = _sharePanel?.IsSubmitting == true ? "Submitting..." : "Submit";
        }
    }

    private void SubmitShare()
    {
        if (_sharingMarkerSet == null || _sharePanel == null || _sharePanel.IsSubmitting)
        {
            return;
        }

        if (!_sharePanel.TryGetCategory(out var category, out var error))
        {
            _sharePanel.SetFeedback(error, null);
            return;
        }

        var markerSet = _sharingMarkerSet;
        _sharePanel.SetSubmitting(true);
        _sharePanel.SetFeedback(null, null);
        ApplyViewMode();

        Task.Run(async () =>
        {
            var result = await CommunityShareHelper.SubmitAsync(markerSet, category).ConfigureAwait(false);
            GameThreadUtil.Enqueue(() =>
            {
                if (_sharePanel == null || _sharingMarkerSet != markerSet)
                {
                    return;
                }

                _sharePanel.SetSubmitting(false);
                if (result.Success)
                {
                    ScreenNotification.ShowNotification(
                        result.Message,
                        ScreenNotification.NotificationType.Green,
                        Service.Textures!._blishHeart,
                        4);
                    CloseShareView();
                    return;
                }

                _sharePanel.SetFeedback(result.Message, null);
                ApplyViewMode();
            });
        });
    }

    private static bool CanShareMarkerSets()
    {
        return Service.Gw2ApiManager != null &&
               Service.Gw2ApiManager.HasPermission(TokenPermission.Account);
    }

    protected void ReloadMarkerList(bool filterToCurrent)
    {
        var currentMapId = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
        _markers = Service.MarkersListing.GetAllMarkerSets();
        RenderLibraryList(_listingPanel, filterToCurrent, currentMapId);
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
            shouldDoIt &= CommanderPermissionHelper.PassesCommanderGate();
        }
        return shouldDoIt;
    }

    protected void RenderLibraryList(FlowPanel? panel, bool shouldFilter, int currentMapId)
    {
        if (panel == null) return;
        int detailButtonWidth = panel.Width - ((int)panel.OuterControlPadding.X * 2) - 10;
        bool showPlaceBtn = CanShowPreviewAndPlaceButtons();

        panel.Children.Clear();

        var searchLower = LibrarySearch.ToLowerCopy(_searchBox?.Text ?? "");

        for (var markerIdx = 0; markerIdx < _markers.Count; markerIdx++)
        {
            var marker = _markers[markerIdx];
            var presetIndex = markerIdx;
            if (shouldFilter && marker.MapId != currentMapId)
            {
                continue;
            }

            if (_mineFilter?.Checked == true && MarkerListing.IsCommunityLinked(marker))
            {
                continue;
            }

            var mapName = marker.MapName;
            if (!LibrarySearch.MatchesLocal(marker, mapName, searchLower))
            {
                continue;
            }
            var communityLinked = MarkerListing.IsCommunityLinked(marker);
            var canShare = !communityLinked &&
                             MarkerListing.IsShareableWithCommunity(marker) &&
                             CanShareMarkerSets();
            var bottomSectionHeight = marker.MapId == currentMapId && showPlaceBtn ? 40 : 35;
            if (communityLinked || canShare)
            {
                bottomSectionHeight = Math.Max(bottomSectionHeight, 40);
            }

            var fallbackIcon = marker.enabled
                ? ((SquadMarker)((markerIdx % 8) + 1)).GetIcon()
                : Service.Textures._imgClear;
            var btn = new DetailsButton()
            {
                Text = (marker.enabled ? "" : "(Disabled) ") + $"{marker.name}\n{marker.description}\n{mapName}",
                Icon = fallbackIcon,
                IconDetails = MarkerListing.DisplayAuthor(marker),
                Width = detailButtonWidth,
                BottomSectionHeight = bottomSectionHeight,
                HighlightType = DetailsHighlightType.LightHighlight,
                ShowToggleButton = true,
                BackgroundColor = marker.enabled ? Color.Transparent : new Color(.4f, .1f, .1f, 0.1f),
            };

            if (!string.IsNullOrWhiteSpace(marker.communitySetId))
            {
                var communitySetId = marker.communitySetId!;
                var thumb = Service.PreviewImageCache.GetThumbTexture(communitySetId, fallbackIcon);
                if (thumb != null)
                {
                    btn.Icon = thumb;
                }
                else
                {
                    var capturedBtn = btn;
                    Service.PreviewImageCache.RequestThumb(communitySetId, "", _ =>
                    {
                        if (capturedBtn.Parent == null)
                        {
                            return;
                        }

                        var loaded = Service.PreviewImageCache.GetThumbTexture(communitySetId, fallbackIcon);
                        if (loaded != null)
                        {
                            capturedBtn.Icon = loaded;
                        }
                    });
                }

                MapPreviewTooltip.Apply(btn, MapPreviewTarget.FromLocalMarker(marker));
            }
            else
            {
                btn.BasicTooltipText = $"{marker.name}\n{marker.description}\nMap: {mapName}\n\nMarkers in use:\n{marker.DescribeMarkers()}";
            }

            var edit = new StandardButton()
            {
                Parent = btn,
                Text = communityLinked ? "Personalize" : "Edit",
                Width = communityLinked ? 95 : 60,
                Location = new Point(10, 5),
                BasicTooltipText = communityLinked
                    ? "Open the editor with this set as a template. Save to add your personalized copy."
                    : $"Click to edit {marker.name}",
                Icon = Service.Textures!.IconEdit
            };
            edit.Click += (s, e) =>
            {
                if (communityLinked)
                {
                    OpenPersonalizedEditor(marker);
                }
                else
                {
                    SwapView(marker, presetIndex);
                }
            };

            if (canShare)
            {
                var share = new StandardButton()
                {
                    Parent = btn,
                    Text = "Share",
                    Width = 75,
                    Location = new Point(edit.Right + 5, 5),
                    BasicTooltipText = "Submit this marker set to the community library",
                    Icon = Service.Textures!.IconImport
                };
                share.Click += (_, __) => OpenShareView(marker);
            }

            if (communityLinked)
            {
                var deleteBtn = new StandardButton()
                {
                    Parent = btn,
                    Text = "Delete",
                    Width = 75,
                    Icon = Service.Textures!.IconDelete,
                    BasicTooltipText = "Remove this imported set from your library"
                };
                deleteBtn.Click += (_, __) =>
                {
                    Service.MapWatch.RemovePreviewMarkerSet();
                    Service.MarkersListing.DeleteMarker(marker);
                    ScreenNotification.ShowNotification(
                        $"Removed \"{marker.name}\" from your library",
                        ScreenNotification.NotificationType.Info);
                };
            }

            if (marker.MapId == currentMapId && showPlaceBtn && marker.enabled)
            {
                var preview = new IconButton()
                {
                    Parent = btn,
                    Icon = Service.Textures!.IconEye,
                    BasicTooltipText = "Hover to preview markers on the map",
                    Size = new Point(30, 30)
                };
                preview.MouseEntered += (s, e) => Service.MapWatch.PreviewMarkerSet(marker);
                preview.MouseLeft += (s, e) => Service.MapWatch.RemovePreviewMarkerSet();
                var placeBtn = new StandardButton()
                {
                    Parent = btn,
                    Icon = Service.Textures!._blishHeartSmall,
                    Text = "Place",
                    Width = 100
                };
                placeBtn.Click += (s, e) => Service.MapWatch.PlaceMarkers(marker);
            }

            var enabledToggle = new EnabledIconButton(marker.enabled)
            {
                Size = new Point(Service.Textures!._imgArrow.Width, Service.Textures._imgArrow.Height),
                Parent = btn,
                Opacity = 0.5f
            };

            enabledToggle.ValueChanged += (_, enabled) =>
            {
                Service.MapWatch.RemovePreviewMarkerSet();
                Service.MarkersListing.SetMarkerEnabled(presetIndex, enabled);
            };

            panel.AddFlowControl(btn);
        }
    }
}
