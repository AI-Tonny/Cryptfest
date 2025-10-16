using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptfest.Data.Entities.WalletEntities;

public class WalletStatistic
{
    public int Id { get; set; }
    [Column(TypeName = "decimal(18,8)")]
    public decimal TotalAssets { get; set; }

    [Column(TypeName = "decimal(18,8)")]
    public decimal TotalDeposit { get; set; }

    [Column(TypeName = "decimal(18,8)")]
    public decimal Apy { get; set; }

}
