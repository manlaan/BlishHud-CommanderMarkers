using Newtonsoft.Json;

namespace Manlaan.CommanderMarkers.Library.Models;

public class CommunityCategoryEntry
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = "";
}
