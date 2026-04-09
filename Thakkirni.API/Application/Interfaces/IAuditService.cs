namespace Thakkirni.API.Application.Interfaces;

public interface IAuditService
{
    Task LogItemEventAsync(int itemId, int userId, string eventType, string description, CancellationToken cancellationToken = default);
}
