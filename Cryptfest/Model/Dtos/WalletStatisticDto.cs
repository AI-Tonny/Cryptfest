using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptfest.Model.Dtos;

public class WalletStatisticDto
{
    public decimal TotalAssets { get; set; }
    public decimal TotalDeposit { get; set; }
    public decimal Apy { get; set; }
}
