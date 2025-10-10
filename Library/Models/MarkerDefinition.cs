using Blish_HUD.Input;
using Manlaan.CommanderMarkers.Library.Enums;

namespace Manlaan.CommanderMarkers.Library.Models;

/// <summary>
/// Defines a marker configuration including its type, display name, and keybindings
/// </summary>
public readonly struct MarkerDefinition
{
    public SquadMarker MarkerType { get; }
    public string DisplayName { get; }
    public bool SupportsGroundTarget { get; }
    public bool SupportsObjectTarget { get; }
    public bool IsClearMarker { get; }

    public MarkerDefinition(
        SquadMarker markerType, 
        string displayName, 
        bool supportsGroundTarget = true, 
        bool supportsObjectTarget = true,
        bool isClearMarker = false)
    {
        MarkerType = markerType;
        DisplayName = displayName;
        SupportsGroundTarget = supportsGroundTarget;
        SupportsObjectTarget = supportsObjectTarget;
        IsClearMarker = isClearMarker;
    }

    /// <summary>
    /// Gets the tooltip text for ground markers
    /// </summary>
    public string GetGroundTooltip() => $"{DisplayName} Ground";

    /// <summary>
    /// Gets the tooltip text for object markers
    /// </summary>
    public string GetObjectTooltip() => $"{DisplayName} Object";

    /// <summary>
    /// Gets the setting key for ground binding
    /// </summary>
    public string GetGroundBindingKey() => $"CmdMrk{DisplayName}GndBinding";

    /// <summary>
    /// Gets the setting key for object binding
    /// </summary>
    public string GetObjectBindingKey() => $"CmdMrk{DisplayName}ObjBinding";
} 