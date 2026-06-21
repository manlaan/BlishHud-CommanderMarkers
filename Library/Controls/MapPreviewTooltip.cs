using Blish_HUD.Common.UI.Views;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Controls;

public sealed class MapPreviewTooltipView : View, ITooltipView
{
    private const int PreviewSize = 768;
    private const int LegendPad = 8;
    private static readonly Color LegendBackground = new(12, 12, 12, 205);

    private readonly MapPreviewTarget _target;
    private Container? _buildPanel;
    private Panel? _previewContainer;
    private Image? _previewImage;
    private FlowPanel? _legendPanel;
    private int _detailFetchGeneration;

    public MapPreviewTooltipView(MapPreviewTarget target)
    {
        _target = target;
    }

    protected override void Build(Container buildPanel)
    {
        if (_buildPanel != null)
        {
            return;
        }

        _buildPanel = buildPanel;

        var root = new FlowPanel
        {
            Parent = buildPanel,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            ControlPadding = new Vector2(0, 4),
            WidthSizingMode = SizingMode.AutoSize,
            HeightSizingMode = SizingMode.AutoSize
        };

        if (!string.IsNullOrWhiteSpace(_target.Label))
        {
            _ = new Label
            {
                Parent = root,
                Text = _target.Label,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                ShowShadow = true
            };
        }

        var description = _target.Description?.Replace("\r\n", "\n").Replace("\r", "\n") ?? "";
        if (!string.IsNullOrWhiteSpace(description))
        {
            _ = new Label
            {
                Parent = root,
                Text = description,
                Width = PreviewSize,
                AutoSizeHeight = true,
                WrapText = true,
                ShowShadow = true
            };
        }

        var hasPreviewImage = !string.IsNullOrEmpty(_target.CommunitySetId);

        _previewContainer = new Panel
        {
            Parent = root,
            Size = new Point(PreviewSize, PreviewSize),
            Visible = hasPreviewImage
        };

        _previewImage = new Image
        {
            Parent = _previewContainer,
            Location = Point.Zero,
            Size = new Point(PreviewSize, PreviewSize),
            Visible = false
        };

        _legendPanel = new FlowPanel
        {
            Parent = hasPreviewImage ? _previewContainer : root,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            ControlPadding = new Vector2(LegendPad, LegendPad),
            WidthSizingMode = SizingMode.AutoSize,
            HeightSizingMode = SizingMode.AutoSize,
            BackgroundColor = LegendBackground,
            Visible = false
        };
        if (hasPreviewImage)
        {
            _legendPanel.Resized += (_, _) => UpdateLegendPosition();
        }

        ApplyPreviewTexture();
        RenderLegend(_target.Markers);
        RequestPreviewIfNeeded();
        RequestDetailIfNeeded();
    }

    private void RequestPreviewIfNeeded()
    {
        if (string.IsNullOrEmpty(_target.CommunitySetId))
        {
            return;
        }

        Service.PreviewImageCache.RequestPreview(_target.CommunitySetId, _target.PreviewLargeUrl, _ =>
        {
            GameThreadUtil.Enqueue(ApplyPreviewTexture);
        });
    }

    private void RequestDetailIfNeeded()
    {
        if (_target.Markers.Count > 0 || string.IsNullOrEmpty(_target.CommunitySetId))
        {
            return;
        }

        var setId = _target.CommunitySetId;
        var generation = ++_detailFetchGeneration;
        Task.Run(() =>
        {
            var fetched = Service.CommunityCatalog.FetchSetDetail(setId);
            if (fetched == null)
            {
                return;
            }

            GameThreadUtil.Enqueue(() =>
            {
                if (generation != _detailFetchGeneration || _legendPanel?.Parent == null)
                {
                    return;
                }

                RenderLegend(fetched.marks);
            });
        });
    }

    private void ApplyPreviewTexture()
    {
        if (_previewImage == null || string.IsNullOrEmpty(_target.CommunitySetId))
        {
            return;
        }

        var texture = Service.PreviewImageCache.GetPreviewTexture(_target.CommunitySetId);
        if (texture != null)
        {
            _previewImage.Texture = texture;
            _previewImage.Visible = true;
            InvalidateLayout();
        }
    }

    private void InvalidateLayout()
    {
        _buildPanel?.Invalidate();
        UpdateLegendPosition();
    }

    private void UpdateLegendPosition()
    {
        if (_legendPanel == null || _previewContainer == null || !_legendPanel.Visible ||
            _legendPanel.Parent != _previewContainer)
        {
            return;
        }

        var legendHeight = _legendPanel.Height;
        var containerHeight = _previewContainer.Height;
        _legendPanel.Location = new Point(
            LegendPad,
            Math.Max(LegendPad, containerHeight - legendHeight - LegendPad));
    }

    private void RenderLegend(IEnumerable<MarkerCoord> markers)
    {
        if (_legendPanel == null)
        {
            return;
        }

        _legendPanel.Children.Clear();
        var entries = markers
            .Where(m => !string.IsNullOrWhiteSpace(m.name) && m.icon != (int)SquadMarker.Clear)
            .OrderBy(m => m.icon)
            .ToList();

        _legendPanel.Visible = entries.Count > 0;
        foreach (var mark in entries)
        {
            var row = new FlowPanel
            {
                Parent = _legendPanel,
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                ControlPadding = new Vector2(6, 0),
                WidthSizingMode = SizingMode.AutoSize,
                Height = 22
            };

            _ = new Image
            {
                Parent = row,
                Size = new Point(18, 18),
                Texture = ((SquadMarker)mark.icon).GetIcon()
            };

            _ = new Label
            {
                Parent = row,
                Text = mark.name,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                TextColor = Color.White,
                ShowShadow = true
            };
        }

        GameThreadUtil.Enqueue(() =>
        {
            if (_legendPanel?.Parent == _previewContainer)
            {
                UpdateLegendPosition();
            }
        });
        InvalidateLayout();
    }
}

public static class MapPreviewTooltip
{
    public static void Apply(Control control, MapPreviewTarget target)
    {
        if (string.IsNullOrWhiteSpace(target.CommunitySetId) &&
            string.IsNullOrWhiteSpace(target.Label))
        {
            return;
        }

        if (control is DetailsButton detailsButton)
        {
            // ScrollingHighlight uses an immediate-mode shader (menuitem.fx) on the
            // DetailsButton that can leak GPU state into the SpriteScreen tooltip pass.
            detailsButton.HighlightType = DetailsHighlightType.LightHighlight;
        }

        control.BasicTooltipText = null;
        control.Tooltip = new Tooltip(new MapPreviewTooltipView(target));
    }
}
