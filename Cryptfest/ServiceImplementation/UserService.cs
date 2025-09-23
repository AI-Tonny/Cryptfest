using API.Data;
using API.Data.Entities.UserEntities;
using Cryptfest.Enums;
using Cryptfest.Interfaces.Services.User;
using Cryptfest.Model.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cryptfest.ServiceImplementation;

public class UserService : IUserService
{
    private readonly ApplicationContext _context;

    public UserService(ApplicationContext context)
    {
        _context = context;
    }

    //public async Task<ToClientDto> ChangeUserDataAsync(int userId, User newUserData)
    //{
    //    User? user = _context.Users.FirstOrDefault(user => user.Id == userId);

    //    if (user == null) {
    //        return new ToClientDto()
    //        {
    //            Message = $"User was not found to change the data",
    //            Status = ResponseStatus.Fail
    //        };
    //    }
    //}

    public async Task<ToClientDto> LoginAsync(UserLogInfo loginUser)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(user => user.UserLogInfoId == loginUser.Id);

        if (user == null)
        {
            return new ToClientDto()
            {
                Message = "This account doesn't exist",
                Status = ResponseStatus.Fail,
                Data = null
            };
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.UserLogInfo.HashPassword, loginUser.HashPassword);

        if (isPasswordValid)
        {
            return new ToClientDto()
            {
                Status = ResponseStatus.Success,
                Data = user
            };
        }
        else
        {
            return new ToClientDto()
            {
                Message = "Password is incorrect",
                Status = ResponseStatus.Fail
            };
        }
    }

    public async Task<ToClientDto> RegisterAsync(UserLogInfo registerUser)
    {
        UserLogInfo? user = await _context.UserLogInfo.FirstOrDefaultAsync(user => user.Login == registerUser.Login);

        if (user != null)
        {
            return new ToClientDto()
            {
                Message = "This login already exists",
                Status = ResponseStatus.Fail,
                Data = null
            };
        }

        registerUser.HashPassword = BCrypt.Net.BCrypt.HashPassword(registerUser.HashPassword);

        User newUser = new User()
        {
            UserLogInfo = registerUser
        };

        await _context.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return new ToClientDto()
        {
            Status = ResponseStatus.Success,
            Data = newUser
        };
    }
}
