using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Services;

public static class MapPreviewTooltipService
{
    private const int PreviewSize = 512;
    private static Panel? _panel;
    private static Label? _titleLabel;
    private static Label? _descriptionLabel;
    private static Image? _previewImage;
    private static FlowPanel? _legendPanel;
    private static string _activeSetId = "";

    public static void Attach(Control anchor, MapPreviewTarget target)
    {
        anchor.MouseEntered += (_, _) => Show(anchor, target);
        anchor.MouseLeft += (_, _) => Hide();
        anchor.Disposed += (_, _) => Hide();
    }

    public static void Hide()
    {
        _activeSetId = "";
        if (_panel != null)
        {
            _panel.Visible = false;
        }
    }

    private static void EnsurePanel()
    {
        if (_panel != null)
        {
            return;
        }

        var screen = GameService.Graphics.SpriteScreen;
        _panel = new Panel
        {
            Parent = screen,
            Size = new Point(PreviewSize + 24, PreviewSize + 120),
            BackgroundColor = new Color(12, 12, 12, 230),
            Visible = false,
            ZIndex = int.MaxValue - 4
        };

        _previewImage = new Image
        {
            Parent = _panel,
            Size = new Point(PreviewSize, PreviewSize),
            Location = new Point(12, 12)
        };

        _titleLabel = new Label
        {
            Parent = _panel,
            Location = new Point(20, 20),
            AutoSizeWidth = true,
            AutoSizeHeight = true,
            TextColor = new Color(245, 245, 245)
        };

        _descriptionLabel = new Label
        {
            Parent = _panel,
            Location = new Point(20, 40),
            Width = PreviewSize - 16,
            AutoSizeHeight = true,
            WrapText = false,
            TextColor = new Color(190, 190, 190)
        };

        _legendPanel = new FlowPanel
        {
            Parent = _panel,
            Location = new Point(12, PreviewSize - 8),
            Width = PreviewSize,
            Height = 80,
            FlowDirection = ControlFlowDirection.SingleLeftToRight,
            ControlPadding = new Vector2(4, 2),
            AutoSizeHeight = true
        };
    }

    private static void Show(Control anchor, MapPreviewTarget target)
    {
        if (string.IsNullOrWhiteSpace(target.CommunitySetId) &&
            string.IsNullOrWhiteSpace(target.Label))
        {
            return;
        }

        EnsurePanel();
        _activeSetId = target.CommunitySetId;
        _panel!.Visible = true;
        PositionNear(anchor);

        _titleLabel!.Text = target.Label;
        _titleLabel.Visible = !string.IsNullOrWhiteSpace(target.Label);

        _descriptionLabel!.Text = target.Description?.Replace("\r\n", "\n").Replace("\r", "\n") ?? "";
        _descriptionLabel.Visible = !string.IsNullOrWhiteSpace(_descriptionLabel.Text);

        var previewPath = !string.IsNullOrEmpty(target.CommunitySetId)
            ? Service.PreviewImageCache.PreviewPathForSet(target.CommunitySetId)
            : null;

        Service.PreviewImageCache.RequestPreview(
            target.CommunitySetId,
            target.PreviewLargeUrl,
            _ =>
            {
                if (_activeSetId != target.CommunitySetId)
                {
                    return;
                }

                GameService.GameThread.Enqueue(() => ApplyPreviewTexture(target.CommunitySetId));
            });

        ApplyPreviewTexture(target.CommunitySetId);
        RenderLegend(target.Markers);

        if (target.Markers.Count == 0 && !string.IsNullOrEmpty(target.CommunitySetId))
        {
            Task.Run(() =>
            {
                var fetched = Service.CommunityCatalog.FetchSetDetail(target.CommunitySetId);
                if (fetched == null || _activeSetId != target.CommunitySetId)
                {
                    return;
                }

                GameService.GameThread.Enqueue(() => RenderLegend(fetched.marks));
            });
        }
    }

    private static void ApplyPreviewTexture(string communitySetId)
    {
        if (_previewImage == null || string.IsNullOrEmpty(communitySetId))
        {
            return;
        }

        var texture = Service.PreviewImageCache.GetPreviewTexture(communitySetId);
        if (texture != null)
        {
            _previewImage.Texture = texture;
        }
    }

    private static void RenderLegend(IEnumerable<MarkerCoord> markers)
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

        foreach (var mark in entries)
        {
            var row = new Panel
            {
                Parent = _legendPanel,
                Width = 140,
                Height = 22,
                BackgroundColor = new Color(12, 12, 12, 180)
            };

            _ = new Image
            {
                Parent = row,
                Size = new Point(18, 18),
                Location = new Point(2, 2),
                Texture = ((SquadMarker)mark.icon).GetIcon()
            };

            _ = new Label
            {
                Parent = row,
                Text = mark.name,
                Location = new Point(24, 3),
                AutoSizeWidth = true,
                TextColor = Color.White
            };
        }
    }

    private static void PositionNear(Control anchor)
    {
        if (_panel == null)
        {
            return;
        }

        var abs = anchor.AbsoluteBounds;
        var x = abs.X + abs.Width + 8;
        var y = abs.Y;
        var screen = GameService.Graphics.SpriteScreen;
        if (x + _panel.Width > screen.Width)
        {
            x = abs.X - _panel.Width - 8;
        }
        if (y + _panel.Height > screen.Height)
        {
            y = Math.Max(8, screen.Height - _panel.Height - 8);
        }

        _panel.Location = new Point(Math.Max(8, x), Math.Max(8, y));
    }
}
