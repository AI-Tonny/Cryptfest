using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptfest.Model.Dtos;

public class CryptoBalanceDto
{
    public decimal Amount { get; set; }
    public decimal Usdt { get; set; }
    public decimal PurchasePrice { get; set; }
    public CryptoAssetDto Asset { get; set; } = default!;
}
