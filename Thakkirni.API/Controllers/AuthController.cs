using Microsoft.AspNetCore.Mvc;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.DTOs;

namespace Thakkirni.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(loginDto, cancellationToken);
        return Ok(response);
    }
}
