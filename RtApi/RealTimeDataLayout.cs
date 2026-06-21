namespace Manlaan.CommanderMarkers.RtApi;

/// <summary>
/// Byte layout for RTAPI shared memory, derived from RTAPI.h.
/// </summary>
public static class RealTimeDataLayout
{
    public const int SquadMarkerSlotCount = 8;
    public const float PlacedMarkerEpsilon = 0.01f;

    public const int GameBuild = 0;
    public const int SquadMarkers = 40;
    public const int SquadMarkerStride = 12;

    public const int RealTimeDataSize = 512;

    public static string DataMapName(int processId) => $"RTAPI_{processId}";
}
