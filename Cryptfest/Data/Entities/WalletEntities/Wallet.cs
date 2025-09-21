using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using System.ComponentModel.DataAnnotations.Schema;
namespace API.Data.Entities.WalletEntities;

public class Wallet
{
    public int Id { get; set; }

    public Guid WalletAddress { get; set; }

    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = new();

    public List<CryptoAssetInfo>? Assets { get; set; }
}
