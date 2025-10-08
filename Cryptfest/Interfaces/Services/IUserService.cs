using Cryptfest.Data.Entities.AuthEntities;
using Cryptfest.Model;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Services;

public interface IUserService
{
    Task<ToClientDto> LoginAsync(LoginRequest loginRequest);
    Task<ToClientDto> RegisterAsync(RegisterRequest registerRequest);
    Task<ToClientDto> ChangePassword(NewPasswordRequest newPasswordRequest, int userId);
    Task<ToClientDto> ChangeLogin(NewLoginRequest newLoginRequest, int userId);
}
