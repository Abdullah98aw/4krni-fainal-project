using Microsoft.AspNetCore.Http;
using Thakkirni.API.Application.Common.Exceptions;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.Data;
using Thakkirni.API.Models;

namespace Thakkirni.API.Application.Services;

public sealed class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogItemEventAsync(int itemId, int userId, string eventType, string description, CancellationToken cancellationToken = default)
    {
        if (itemId <= 0)
            throw new ApiException(StatusCodes.Status400BadRequest, "معرّف العنصر غير صالح", null);
        if (userId <= 0)
            throw new ApiException(StatusCodes.Status400BadRequest, "معرّف المستخدم غير صالح", null);
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ApiException(StatusCodes.Status400BadRequest, "نوع الحدث مطلوب", null);

        _context.AuditEvents.Add(new AuditEvent
        {
            ItemId = itemId,
            UserId = userId,
            Type = eventType.Trim().ToUpperInvariant(),
            MetaData = string.IsNullOrWhiteSpace(description) ? eventType.Trim() : description.Trim(),
            CreatedAt = DateTime.UtcNow
        });
    }
}
