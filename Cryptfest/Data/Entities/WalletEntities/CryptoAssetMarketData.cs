using API.Data.Entities.Wallet;
using Cryptfest.Data.Entities.WalletEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.WalletEntities;

public class CryptoAssetMarketData
{
    public int Id { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal CurrPrice { get; set; }
    [Column(TypeName = "decimal(10, 2)")]
    public decimal PercentChange1h { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PercentChange24h { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PercentChange7d { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PercentChange30d { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PercentChange60d { get; set; }
}
