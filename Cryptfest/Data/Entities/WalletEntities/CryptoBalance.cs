using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptfest.Data.Entities.WalletEntities;

public class CryptoBalance
{
    public int Id { get; set; }

    [Column(TypeName = "decimal(18, 8)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(18, 8)")]
    public decimal Usdt { get; set; }

    [Column(TypeName = "decimal(18, 8)")]
    public decimal PurchasePrice { get; set; }

    public int WalletId { get; set; }
    [ForeignKey(nameof(WalletId))]
    public Wallet Wallet { get; set; } = default!;

    public int AssetId { get; set; }
    [ForeignKey(nameof(AssetId))]
    public CryptoAsset Asset { get; set; } = default!;
}
