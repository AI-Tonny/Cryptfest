using API.Data.Entities.WalletEntities;
using System.Text.Json.Serialization;

namespace API.Model.Dtos;

public class GetListOfAssetsDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("resultset_size")]
    public int Size { get; set; }

    [JsonPropertyName("result")]
    public List<CryptoAssetDto> Result { get; set; } = default!;
}
