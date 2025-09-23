using API.Data.Entities.WalletEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.Wallet;

public class CryptoAssetInfo
{
    public int Id { get; set; }
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public string Logo { get; set; } = "";
    public int? PriceId { get; set; }
    [ForeignKey(nameof(PriceId))]
    public CryptoAssetPrice? Price { get; set; }
}

