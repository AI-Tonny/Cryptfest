using Cryptfest.Data.Entities.AuthEntities;
using Cryptfest.Model;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Services;

public interface IUserService
{
    Task<ToClientDto> LoginAsync(LoginRequest loginRequest);
    Task<ToClientDto> RegisterAsync(RegisterRequest registerRequest);
    Task<ToClientDto> ChangePassword(PasswordRequest passwordRequest, int userId);
}
