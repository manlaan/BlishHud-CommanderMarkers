namespace Manlaan.CommanderMarkers.Utils;

/// <summary>
/// Centralized constants for timing delays and magic numbers used throughout the application
/// </summary>
public static class Constants
{
    // Timing delays (in milliseconds)
    public static class Delays
    {
        /// <summary>Standard delay for hotkey operations</summary>
        public const int HOTKEY_DELAY_MS = 50;
        
        /// <summary>Delay for mouse position updates</summary>
        public const int MOUSE_POSITION_DELAY_MS = 20;
        
        /// <summary>Delay for marker placement operations</summary>
        public const int MARKER_PLACEMENT_DELAY_MS = 60;
        
        /// <summary>Delay for input operations</summary>
        public const int INPUT_DELAY_MS = 10;
        
        /// <summary>Delay for map watch operations (half of total delay)</summary>
        public const int MAP_WATCH_HALF_DELAY_MS = 100;
    }

    // UI Constants
    public static class UI
    {
        /// <summary>Default icon size for marker buttons</summary>
        public const int DEFAULT_ICON_SIZE = 30;
        
        /// <summary>Default opacity for UI elements</summary>
        public const float DEFAULT_OPACITY = 1.0f;
        
        /// <summary>Minimum icon size</summary>
        public const int MIN_ICON_SIZE = 16;
        
        /// <summary>Maximum icon size</summary>
        public const int MAX_ICON_SIZE = 200;
        
        /// <summary>Minimum opacity value</summary>
        public const float MIN_OPACITY = 0.1f;
        
        /// <summary>Maximum opacity value</summary>
        public const float MAX_OPACITY = 1.0f;
    }

    // AutoMarker Constants
    public static class AutoMarker
    {
        /// <summary>Default placement delay for auto markers</summary>
        public const int DEFAULT_PLACEMENT_DELAY_MS = 100;
        
        /// <summary>Minimum placement delay</summary>
        public const int MIN_PLACEMENT_DELAY_MS = 50;
        
        /// <summary>Maximum placement delay</summary>
        public const int MAX_PLACEMENT_DELAY_MS = 300;
    }

    // Corner Icon Constants
    public static class CornerIcon
    {
        /// <summary>Default priority for corner icon</summary>
        public const int DEFAULT_PRIORITY = 478;
        
        /// <summary>Minimum priority value</summary>
        public const int MIN_PRIORITY = 0;
        
        /// <summary>Maximum priority value</summary>
        public const int MAX_PRIORITY = 1000;
    }
} 