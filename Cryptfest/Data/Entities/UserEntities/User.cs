using API.Data.Entities.WalletEntities;
using Cryptfest.Data.Entities.ClientRequest;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.UserEntities;

public class User
{
    public int Id { get; set; }

    public int UserLogInfoId { get; set; }
    [ForeignKey(nameof(UserLogInfoId))]
    public UserLogInfo UserLogInfo { get; set; } = default!;

    public ClientRequest ClientRequest { get; set; } = new();

    public DateTime CreatedDate { get; set; }
}
