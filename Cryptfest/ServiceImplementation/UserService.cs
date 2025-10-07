using API.Data;
using API.Data.Entities.UserEntities;
using Cryptfest.Data.Entities.AuthEntities;
using Cryptfest.Enums;
using Cryptfest.Interfaces.Services;
using Cryptfest.Interfaces.Validation;
using Cryptfest.Model;
using Cryptfest.Model.Dtos;
using Cryptfest.ServiceImpementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cryptfest.ServiceImplementation;

public class UserService : IUserService
{
    private readonly ApplicationContext _context;

    private readonly IUserValidation _userValidation;
    private readonly IConfiguration _configuration;
    private readonly ICryptoService _cryptoService;

    public UserService(ApplicationContext context, IUserValidation userValidation, IConfiguration configuration, ICryptoService cryptoService)
    {
        _context = context;
        _userValidation = userValidation;
        _cryptoService = cryptoService;
        _configuration = configuration;
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

    public async Task<ToClientDto> LoginAsync(LoginRequest loginRequest)
    {
        ValidationResult isLoginValid = _userValidation.IsLoginValid(loginRequest.Login);
        ValidationResult isPasswordValid = _userValidation.IsPasswordValid(loginRequest.Password);

        if (!isLoginValid.isValid || !isPasswordValid.isValid)
        {
            return new ToClientDto()
            {
                Message = isLoginValid.isValid ? isPasswordValid.Message : isLoginValid.Message,
                Status = ResponseStatus.Fail,
            };
        }

        User? user = await FindUserByLoginAsync(loginRequest.Login);

        if (user == null)
        {
            return new ToClientDto()
            {
                Message = "This account does not exist",
                Status = ResponseStatus.Fail,
            };
        }

        bool isHashPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.UserLogInfo.HashPassword);

        if (isHashPasswordValid)
        {
            return new ToClientDto()
            {
                Status = ResponseStatus.Success,
                Data = GenerateJwtToken(user)
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

    public async Task<ToClientDto> RegisterAsync(RegisterRequest registerRequest)
    {
        ValidationResult isLoginValid = _userValidation.IsLoginValid(registerRequest.Login);
        ValidationResult isPasswordValid = _userValidation.IsPasswordValid(registerRequest.Password);

        if (!isLoginValid.isValid || !isPasswordValid.isValid)
        {
            return new ToClientDto()
            {
                Message = isLoginValid.isValid ? isPasswordValid.Message : isLoginValid.Message,
                Status = ResponseStatus.Fail
            };
        }

        UserLogInfo? user = await FindUserLogInfoByLoginAsync(registerRequest.Login);

        if (user != null)
        {
            return new ToClientDto()
            {
                Message = "This login is already taken",
                Status = ResponseStatus.Fail
            };
        }

        UserLogInfo registerUser = new UserLogInfo()
        {
            Login = registerRequest.Login,
            Email = registerRequest.Email,
            HashPassword = registerRequest.Password
        };

        registerUser.HashPassword = BCrypt.Net.BCrypt.HashPassword(registerUser.HashPassword);

        User newUser = new User()
        {
            UserLogInfo = registerUser,
            UserPersonalInfo = new UserPersonalInfo(),
            CreatedDate = DateTime.Now
        };

        await _cryptoService.CreateWallet(newUser);

        return new ToClientDto()
        {
            Status = ResponseStatus.Success
        };
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserPersonalInfo.Name)
            }),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
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
