using System.ComponentModel.DataAnnotations;

namespace Cryptfest.Data.Entities.AuthEntities;

public class RegisterRequest
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
}
