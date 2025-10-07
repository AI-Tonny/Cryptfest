namespace API.Data.Entities.UserEntities;

public class UserPersonalInfo
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";

    public string Avatar { get; set; } = "";
}
