using Microsoft.AspNetCore.Mvc;
using FlashCard.Api.Models;
using FlashCard.Api.Services;

namespace FlashCard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(model);

        if (!result.Successful)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(model);

        if (!result.Successful)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
} 