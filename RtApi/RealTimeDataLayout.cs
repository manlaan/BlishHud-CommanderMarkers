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
    public const int GroupType = 136;

    public const int RealTimeDataSize = 512;

    public static string DataMapName(int processId) => $"RTAPI_{processId}";
    public static string EventsMapName(int processId) => $"RTAPIEvents_{processId}";
    public static string EventSignalName(int processId) => $"RTAPIEvent_{processId}";

    public enum GroupTypeValue : uint
    {
        None = 0,
        Party = 1,
        RaidSquad = 2,
        Squad = 3,
    }

    public enum RtApiEventKind : uint
    {
        GroupMemberJoined = 1,
        GroupMemberLeft = 2,
        GroupMemberUpdated = 3,
    }

    public const int EventsHeaderSize = 16;
    public const int EventKindSize = 4;
    public const int GroupMemberSize = 296;
    public const int EventEntrySize = EventKindSize + GroupMemberSize;
    public const int MaxEventSlots = 32;
    public const int EventsMapSize = EventsHeaderSize + (EventEntrySize * MaxEventSlots);

    public const int GroupMemberAccountName = 0;
    public const int GroupMemberCharacterName = 140;
    public const int GroupMemberSubgroup = 280;
    public const int GroupMemberProfession = 284;
    public const int GroupMemberEliteSpecialization = 288;
    public const int GroupMemberFlags = 292;

    public const uint FlagIsSelf = 1u << 0;
    public const uint FlagIsInInstance = 1u << 1;
    public const uint FlagIsCommander = 1u << 2;
    public const uint FlagIsLieutenant = 1u << 3;
}
