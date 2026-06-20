using Newtonsoft.Json;

namespace Manlaan.CommanderMarkers.Library.Models;

public class CommunitySetSummary
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("categoryId")]
    public int CategoryId { get; set; }

    [JsonProperty("categoryName")]
    public string CategoryName { get; set; } = "";

    [JsonProperty("author")]
    public string Author { get; set; } = "";

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("description")]
    public string Description { get; set; } = "";

    [JsonProperty("mapId")]
    public int MapId { get; set; }

    [JsonProperty("mapName")]
    public string MapName { get; set; } = "";

    [JsonProperty("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonProperty("previewThumbUrl")]
    public string PreviewThumbUrl { get; set; } = "";

    [JsonProperty("updatedAt")]
    public string UpdatedAt { get; set; } = "";
}
