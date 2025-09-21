using API.Data.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.WalletEntities;

public class CryptoTransfer
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int CryptoId { get; set; }
    public CryptoAssetInfo Crypto { get; set; } = default!;

    public int FromWalletId { get; set; }
    [ForeignKey(nameof(FromWalletId))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public Wallet FromWallet { get; set; } = default!;

    public int ToWalletId { get; set; }
    [ForeignKey(nameof(ToWalletId))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public Wallet ToWallet { get; set; } = default!;

    public DateTime Date { get; set; } = DateTime.UtcNow;
    public TradeType Type { get; set; }
}
