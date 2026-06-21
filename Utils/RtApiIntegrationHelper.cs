namespace Manlaan.CommanderMarkers.Utils;

public static class RtApiIntegrationHelper
{
    public static bool IsEnabled => Service.Settings.RtApiIntegrationEnabled.Value;
}
