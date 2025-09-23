using API.Data.Entities.UserEntities;
using Cryptfest.Interfaces.Services.User;
using Cryptfest.Model.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Cryptfest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("log-in")]
    public async Task<IActionResult> Login(UserLogInfo userLogInfo)
    {
        return Ok(await _userService.LoginAsync(userLogInfo));
    }

    [HttpPost("create-account")]
    public async Task<IActionResult> Register(UserLogInfo userLogInfo)
    {
        return Ok(await _userService.RegisterAsync(userLogInfo));
    }
}
