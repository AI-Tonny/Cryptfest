using API.Data.Entities.UserEntities;
using Cryptfest.Interfaces.Services;
using Cryptfest.Data.Entities.AuthEntities;
using Cryptfest.Interfaces.Services;
using Cryptfest.Model;
using Cryptfest.Model.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Cryptfest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    public UserController(IUserService userService, IEmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    [HttpPost("log-in")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        return Ok(await _userService.LoginAsync(loginRequest));
    }

    [HttpPost("create-account")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        return Ok(await _userService.RegisterAsync(registerRequest));
    }

    [HttpPost("send-verification-code")]
    public async Task<IActionResult> sendVerificationCode([FromBody] VerificationRequest verificationRequest)
    {
        return Ok(await _emailService.SendVerificationEmail(verificationRequest));
    }
}
