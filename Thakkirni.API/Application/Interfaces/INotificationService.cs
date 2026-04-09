using Thakkirni.API.Models;

namespace Thakkirni.API.Application.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<Notification>> GetNotificationsAsync(int userId, CancellationToken cancellationToken = default);
    Task MarkReadAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(int userId, CancellationToken cancellationToken = default);
    Task CreateAsync(int userId, string title, string body, CancellationToken cancellationToken = default);
}
