using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptfest.Model.Dtos;

public class CryptoAssetMarketDataDto
{
    public decimal CurrPrice { get; set; }
    //public decimal PercentChange1h { get; set; }
    //public decimal PercentChange24h { get; set; }
    //public decimal PercentChange7d { get; set; }
    //public decimal PercentChange30d { get; set; }
    //public decimal PercentChange60d { get; set; }
}
