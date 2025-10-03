using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using Cryptfest.Data.Entities.WalletEntities;
using System.ComponentModel.DataAnnotations.Schema;
namespace API.Data.Entities.WalletEntities;

public class Wallet
{
    public int Id { get; set; }

    public int StatisticId { get; set; }
    [ForeignKey(nameof(StatisticId))]
    public WalletStatistic Statistic { get; set; } = new();

    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = new();

    public List<CryptoBalance> Balances { get; set; } = new();
}
