namespace Manlaan.CommanderMarkers.RtApi;

public static class RtApiStatusText
{
    public static string ForState(RtApiConnectionState state)
    {
        return state switch
        {
            RtApiConnectionState.Active => "RTAPI: active",
            RtApiConnectionState.Inactive => "RTAPI: detected (inactive)",
            _ => "RTAPI: not detected",
        };
    }
}
