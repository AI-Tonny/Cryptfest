using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.Wallet;

public class CryptoTrade
{
    public int Id { get; set; }
    public int CryptoInfoId { get; set; }
    [ForeignKey(nameof(CryptoInfoId))]
    public CryptoAssetInfo CryptoInfo {  get; set; } = default!;
    public decimal Price { get; set; }  
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public TradeType Type { get; set; }
}
