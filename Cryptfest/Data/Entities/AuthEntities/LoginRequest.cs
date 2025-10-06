namespace Cryptfest.Data.Entities.AuthEntities;

public class LoginRequest
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
}
