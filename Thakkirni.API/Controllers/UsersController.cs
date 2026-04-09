using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.DTOs;

namespace Thakkirni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(IUserService userService, ICurrentUserService currentUserService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        => Ok(await _userService.GetUsersAsync(cancellationToken));

    [HttpGet("team")]
    [Authorize(Roles = "ADMIN,MANAGER")]
    public async Task<IActionResult> GetScopedUsers(CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _userService.GetScopedUsersAsync(currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        => Ok(await _userService.GetUserByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUpdateUserDto dto, CancellationToken cancellationToken)
        => Ok(await _userService.CreateUserAsync(dto, cancellationToken));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUpdateUserDto dto, CancellationToken cancellationToken)
        => Ok(await _userService.UpdateUserAsync(id, dto, cancellationToken));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        await _userService.DeleteUserAsync(id, currentUser.UserId, cancellationToken);
        return NoContent();
    }
}
