using Cryptfest.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.Wallet;

public class CryptoTransaction
{
    public int Id { get; set; }

    public DateTime Date { get; set; }
    [Column(TypeName = "decimal(18, 8)")]
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }

    public int? FromAssetId { get; set; }
    [ForeignKey(nameof(FromAssetId))]
    public CryptoAsset? FromAsset {  get; set; } = default!;

    public int? ToAssetId { get; set; }
    [ForeignKey(nameof(ToAssetId))]
    public CryptoAsset? ToAsset { get; set; } = default!;
}
