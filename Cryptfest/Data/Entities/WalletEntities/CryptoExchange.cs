using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.Wallet;

public class CryptoExchange
{
    public int Id { get; set; }
    public int CryptoInfoId { get; set; }
    [ForeignKey(nameof(CryptoInfoId))]
    public CryptoAsset CryptoInfo {  get; set; } = default!;
    public DateTime Date { get; set; }
    [Column(TypeName = "decimal(18, 8)")]
    public decimal Amount { get; set; }
    public string FromAssset { get; set; } = "";
    public string ToAssset { get; set; } = "";
}
