using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Thakkirni.API.Application.Common.Exceptions;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.Data;
using Thakkirni.API.Models;

namespace Thakkirni.API.Application.Services;

public sealed class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkReadAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var affected = await _context.Notifications
            .Where(n => n.Id == id && n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsRead, true), cancellationToken);

        if (affected == 0)
        {
            var exists = await _context.Notifications.AnyAsync(n => n.Id == id && n.UserId == userId, cancellationToken);
            if (!exists)
                throw new ApiException(StatusCodes.Status404NotFound, "الإشعار غير موجود", new { message = "الإشعار غير موجود" });
        }
    }

    public async Task MarkAllReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsRead, true), cancellationToken);
    }

    public async Task CreateAsync(int userId, string title, string body, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Add(new Notification
        {
            UserId = userId,
            Title = title,
            Body = body,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);
    }
}
