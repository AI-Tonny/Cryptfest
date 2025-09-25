using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities.UserEntities;

public class User
{
    public int Id { get; set; }

    public int UserPersonalInfoId { get; set; }
    [ForeignKey(nameof(UserPersonalInfoId))]
    public UserPersonalInfo UserPersonalInfo { get; set; } = default!;

    public int UserLogInfoId { get; set; }
    [ForeignKey(nameof(UserLogInfoId))]
    public UserLogInfo UserLogInfo { get; set; } = default!;

    public DateTime CreatedDate { get; set; }
}
