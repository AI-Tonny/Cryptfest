using API.Data;
using API.Data.Entities.UserEntities;
using Cryptfest.Enums;
using Cryptfest.Interfaces.Services;
using Cryptfest.Interfaces.Validation;
using Cryptfest.Model;
using Cryptfest.Model.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Cryptfest.ServiceImplementation;

public class UserService : IUserService
{
    private readonly ApplicationContext _context;
    private readonly IUserValidation _userValidation;

    public UserService(ApplicationContext context, IUserValidation userValidation)
    {
        _context = context;
        _userValidation = userValidation;
    }

    public async Task<ToClientDto> LoginAsync(UserLogInfo loginUser)
    {
        ValidationResult isLoginValid = _userValidation.IsLoginValid(loginUser.Login);
        ValidationResult isPasswordValid = _userValidation.IsPasswordValid(loginUser.HashPassword);

        if (!isLoginValid.isValid || !isPasswordValid.isValid)
        {
            return new ToClientDto()
            {
                Message = isLoginValid.isValid ? isPasswordValid.Message : isLoginValid.Message,
                Status = ResponseStatus.Fail,
            };
        }

        User? user = await FindUserByLoginAsync(loginUser.Login);

        if (user == null)
        {
            return new ToClientDto()
            {
                Message = "This account does not exist",
                Status = ResponseStatus.Fail,
            };
        }

        bool isHashPasswordValid = BCrypt.Net.BCrypt.Verify(loginUser.HashPassword, user.UserLogInfo.HashPassword);

        if (isHashPasswordValid)
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
        ValidationResult isLoginValid = _userValidation.IsLoginValid(registerUser.Login);
        ValidationResult isPasswordValid = _userValidation.IsPasswordValid(registerUser.HashPassword);

        if (!isLoginValid.isValid || !isPasswordValid.isValid)
        {
            return new ToClientDto()
            {
                Message = isLoginValid.isValid ? isPasswordValid.Message : isLoginValid.Message,
                Status = ResponseStatus.Fail
            };
        }

        UserLogInfo? user = await FindUserLogInfoByLoginAsync(registerUser.Login);

        if (user != null)
        {
            return new ToClientDto()
            {
                Message = "This login is already taken",
                Status = ResponseStatus.Fail
            };
        }

        registerUser.HashPassword = BCrypt.Net.BCrypt.HashPassword(registerUser.HashPassword);

        User newUser = new User()
        {
            UserLogInfo = registerUser,
            UserPersonalInfo = new UserPersonalInfo(),
            CreatedDate = DateTime.Now
        };

        await _context.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return new ToClientDto()
        {
            Status = ResponseStatus.Success,
            Data = newUser
        };
    }

    public async Task<ToClientDto> ChangeUserDataAsync(int userId, UserPersonalInfo newUserData)
    {
        User? user = await FindUserByIdAsync(userId);

        if (user == null)
        {
            return new ToClientDto()
            {
                Message = $"User was not found to change the data",
                Status = ResponseStatus.Fail
            };
        }
    }

    public async Task<User?> FindUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(user => user.UserLogInfo)
            .Include(user => user.UserPersonalInfo)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> FindUserByLoginAsync(string login)
    {
        return await _context.Users
            .Include(user => user.UserLogInfo)
            .Include(user => user.UserPersonalInfo)
            .FirstOrDefaultAsync(user => user.UserLogInfo.Login == login);
    }

    public async Task<UserLogInfo?> FindUserLogInfoByLoginAsync(string login)
    {
        return await _context.UserLogInfo
            .FirstOrDefaultAsync(userLogInfo => userLogInfo.Login == login);
    }
}
