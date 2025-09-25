using API.Data.Entities.UserEntities;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Services.User;

public interface IUserService
{
    Task<ToClientDto> LoginAsync(UserLogInfo logUser);
    Task<ToClientDto> RegisterAsync(UserLogInfo registerUser);
}
