using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
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
=======
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Thakkirni.API.Data;

namespace Thakkirni.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();
            return Ok(notifications);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (notif == null) return NotFound();
            notif.IsRead = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var notifs = await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
            foreach (var n in notifs) n.IsRead = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    }
}
