using System.Runtime.CompilerServices;

namespace API.Data.Entities.UserEntities;

public class UserLogInfo
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string HashPassword { get; set; } = "";
}
