using System.Text.Json.Serialization;

namespace API.Model.Dtos;

public class CryptoAssetDto
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("source")]
    public string Source { get; set; } = "";
}
