using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TokenPermission = Gw2Sharp.WebApi.V2.Models.TokenPermission;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class MarkerSetSharePanel : FlowPanel
{
    private MarkerSet _markerSet = new();
    private Dropdown? _categoryDropdown;
    private TextBox? _customCategoryBox;
    private Label? _errorLabel;
    private Label? _statusLabel;
    private List<string> _categoryNames = new();

    public bool IsSubmitting { get; private set; }

    public MarkerSetSharePanel()
    {
        ControlPadding = new Vector2(5, 5);
        FlowDirection = ControlFlowDirection.SingleTopToBottom;
    }

    public void LoadMarkerSet(MarkerSet markerSet)
    {
        _markerSet = markerSet;
        IsSubmitting = false;
        ClearChildren();
        BuildContent();
    }

    public void SetSubmitting(bool submitting)
    {
        IsSubmitting = submitting;
        if (_categoryDropdown != null)
        {
            _categoryDropdown.Enabled = !submitting;
        }

        if (_customCategoryBox != null)
        {
            _customCategoryBox.Enabled = !submitting;
        }
    }

    public void SetFeedback(string? error, string? status)
    {
        if (_errorLabel != null)
        {
            _errorLabel.Text = error ?? "";
            _errorLabel.Visible = !string.IsNullOrWhiteSpace(error);
        }

        if (_statusLabel != null)
        {
            _statusLabel.Text = status ?? "";
            _statusLabel.Visible = !string.IsNullOrWhiteSpace(status);
        }
    }

    public bool TryGetCategory(out string category, out string? error)
    {
        var categoryIndex = _categoryDropdown?.Items.IndexOf(_categoryDropdown.SelectedItem) ?? -1;
        var customCategory = _customCategoryBox?.Text ?? "";
        category = CommunityShareHelper.ResolveCategory(categoryIndex, customCategory, _categoryNames);
        if (string.IsNullOrWhiteSpace(category))
        {
            error = "Enter a category name.";
            return false;
        }

        error = null;
        return true;
    }

    private void BuildContent()
    {
        _ = new Label
        {
            Parent = this,
            Text = "Share this marker set with the community!",
            AutoSizeWidth = true,
            AutoSizeHeight = true
        };

        _ = new Label
        {
            Parent = this,
            Text = "Check your marker set text and choose a category. Submissions are reviewed prior to publishing.",
            Width = Width - 40,
            WrapText = true,
            AutoSizeHeight = true
        };

        if (Service.Gw2ApiManager == null || !Service.Gw2ApiManager.HasPermission(TokenPermission.Account))
        {
            _ = new Label
            {
                Parent = this,
                Text = "Enable the Account API permission for Commander Markers in BlishHUD to share marker sets.",
                Width = Width - 40,
                WrapText = true,
                AutoSizeHeight = true
            };
            return;
        }

        AddReadOnlyField("Name", _markerSet.name ?? "");
        AddReadOnlyField("Description", _markerSet.description ?? "");
        AddReadOnlyField("Map", _markerSet.MapName);
        AddReadOnlyField("Markers", (_markerSet.marks?.Count ?? 0).ToString());

        _categoryNames = CommunityShareHelper.CategoryNames();
        var customIndex = _categoryNames.Count;

        var categoryRow = new FlowPanel
        {
            Parent = this,
            FlowDirection = ControlFlowDirection.LeftToRight,
            ControlPadding = new Vector2(10, 5),
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize
        };

        _ = new Label
        {
            Parent = categoryRow,
            Text = "Category",
            Width = 100,
            Height = 30
        };

        _categoryDropdown = new Dropdown
        {
            Parent = categoryRow,
            Width = 280,
            Height = 30
        };
        foreach (var name in _categoryNames)
        {
            _categoryDropdown.Items.Add(name);
        }

        _categoryDropdown.Items.Add("Suggest New Category...");
        _categoryDropdown.SelectedItem = _categoryNames.Count == 0
            ? _categoryDropdown.Items[customIndex]
            : _categoryDropdown.Items[0];

        _customCategoryBox = new TextBox
        {
            Parent = categoryRow,
            Width = 280,
            BasicTooltipText = "Type category name",
            Visible = _categoryNames.Count == 0
        };

        _categoryDropdown.ValueChanged += (_, __) =>
        {
            if (_categoryDropdown == null || _customCategoryBox == null)
            {
                return;
            }

            var selectedIndex = _categoryDropdown.Items.IndexOf(_categoryDropdown.SelectedItem);
            _customCategoryBox.Visible = selectedIndex == customIndex || _categoryNames.Count == 0;
        };

        _errorLabel = new Label
        {
            Parent = this,
            Width = Width - 40,
            WrapText = true,
            AutoSizeHeight = true,
            Visible = false
        };

        _statusLabel = new Label
        {
            Parent = this,
            Width = Width - 40,
            WrapText = true,
            AutoSizeHeight = true,
            Visible = false
        };
    }

    private void AddReadOnlyField(string label, string value)
    {
        var row = new FlowPanel
        {
            Parent = this,
            FlowDirection = ControlFlowDirection.SingleLeftToRight,
            ControlPadding = new Vector2(10, 5),
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize
        };

        _ = new Label
        {
            Parent = row,
            Text = label,
            Width = 100,
            Height = 30
        };

        _ = new Label
        {
            Parent = row,
            Text = value,
            Width = 400,
            WrapText = true,
            AutoSizeHeight = true
        };
    }
}
