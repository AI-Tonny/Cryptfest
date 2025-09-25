using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.Wallet;

public class CryptoTrade
{
    public int Id { get; set; }
    public int CryptoInfoId { get; set; }
    [ForeignKey(nameof(CryptoInfoId))]
    public CryptoAsset CryptoInfo {  get; set; } = default!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }  
    public DateTime Date { get; set; }
    [Column(TypeName = "decimal(18, 8)")]
    public decimal Amount { get; set; }
    public TradeType Type { get; set; }
}
