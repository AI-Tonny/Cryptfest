using API.Data.Entities.UserEntities;
using Cryptfest.Data.Entities.WalletEntities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cryptfest.Model.Dtos;

public class WalletDto
{
    [JsonPropertyName("walletId")]
    public Guid Id { get; set; }

    public WalletStatisticDto Statistic { get; set; } = new();
    //public User User { get; set; } = new();
    public List<CryptoBalanceDto> Balances { get; set; } = new();
}
