using API.Data.Entities.WalletEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptfest.Model.Dtos;

public class CryptoAssetDto
{
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public string Logo { get; set; } = "";
    public CryptoAssetMarketDataDto MarketData { get; set; } = default!;
}
