using Blish_HUD.Input;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Library.Models;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Manlaan.CommanderMarkers.Library.Services;

/// <summary>
/// Service that provides centralized access to marker definitions and their configurations
/// </summary>
public static class MarkerDefinitionService
{
    /// <summary>
    /// All available marker definitions in the order they should be displayed
    /// </summary>
    public static readonly MarkerDefinition[] AllMarkers = {
        new(SquadMarker.Arrow, "Arrow"),
        new(SquadMarker.Circle, "Circle"),
        new(SquadMarker.Heart, "Heart"),
        new(SquadMarker.Square, "Square"),
        new(SquadMarker.Star, "Star"),
        new(SquadMarker.Spiral, "Spiral"),
        new(SquadMarker.Triangle, "Triangle"),
        new(SquadMarker.Cross, "X"),
        new(SquadMarker.Clear, "Clear", true, true, true)
    };

    /// <summary>
    /// Gets the default keybinding for a marker type and target
    /// </summary>
    public static KeyBinding GetDefaultKeyBinding(SquadMarker markerType, bool isGroundTarget)
    {
        var key = markerType switch
        {
            SquadMarker.Arrow => Keys.D1,
            SquadMarker.Circle => Keys.D2,
            SquadMarker.Heart => Keys.D3,
            SquadMarker.Square => Keys.D4,
            SquadMarker.Star => Keys.D5,
            SquadMarker.Spiral => Keys.D6,
            SquadMarker.Triangle => Keys.D7,
            SquadMarker.Cross => Keys.D8,
            SquadMarker.Clear => Keys.D9,
            _ => Keys.D1
        };

        var modifier = isGroundTarget ? ModifierKeys.Alt : ModifierKeys.Alt | ModifierKeys.Shift;
        return new KeyBinding(modifier, key);
    }

    /// <summary>
    /// Gets a marker definition by its type
    /// </summary>
    public static MarkerDefinition? GetByType(SquadMarker markerType)
    {
        return AllMarkers.FirstOrDefault(m => m.MarkerType == markerType);
    }

    /// <summary>
    /// Gets all markers that support ground targeting
    /// </summary>
    public static IEnumerable<MarkerDefinition> GetGroundMarkers()
    {
        return AllMarkers.Where(m => m.SupportsGroundTarget);
    }

    /// <summary>
    /// Gets all markers that support object targeting
    /// </summary>
    public static IEnumerable<MarkerDefinition> GetObjectMarkers()
    {
        return AllMarkers.Where(m => m.SupportsObjectTarget);
    }
} 