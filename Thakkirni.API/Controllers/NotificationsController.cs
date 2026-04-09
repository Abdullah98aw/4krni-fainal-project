using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thakkirni.API.Application.Interfaces;

namespace Thakkirni.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(INotificationService notificationService, ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _notificationService.GetNotificationsAsync(currentUser.UserId, cancellationToken));
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkRead(int id, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        await _notificationService.MarkReadAsync(id, currentUser.UserId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        await _notificationService.MarkAllReadAsync(currentUser.UserId, cancellationToken);
        return NoContent();
    }
}
