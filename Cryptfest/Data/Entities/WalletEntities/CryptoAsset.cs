using API.Data.Entities.WalletEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.Wallet;

public class CryptoAsset
{
    public int Id { get; set; }
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public string Logo { get; set; } = "";
    public int MarketDataId { get; set; }
    [ForeignKey(nameof(MarketDataId))]
    public CryptoAssetMarketData MarketData { get; set; } = default!;
}

