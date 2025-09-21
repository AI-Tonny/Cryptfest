using API.Data.Entities.Wallet;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.WalletEntities;

public class CryptoAssetPrice
{
    public int Id { get; set; }
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }
}
