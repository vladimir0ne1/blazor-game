using BlazorGame.Server.Data;
using BlazorGame.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGame.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        this.authRepository = authRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegister request)
    {
        var newUser = new User
        {
            Bananas = request.Bananas,
            Username = request.UserName,
            DateOfBirth = request.DateOfBirth,
            IsConfirmed = request.IsConfirmed,
            Email = request.Email
        };

        var res = await authRepository.Register(newUser, request.Password, request.StartUnitId);

        if (!res.Success)
        {
            return BadRequest(res);
        }

        return Ok(res);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLogin request)
    {
        var res = await authRepository.Login(request.Email, request.Password);

        if (!res.Success)
        {
            return BadRequest(res);
        }

        return Ok(res);
    }
}
