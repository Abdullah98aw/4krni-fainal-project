using Thakkirni.API.Application.Common.Models;
using Thakkirni.API.DTOs;

namespace Thakkirni.API.Application.Interfaces;

public interface IItemService
{
    Task<IReadOnlyList<ItemDto>> GetItemsAsync(int currentUserId, string currentRole, int page, int pageSize, string? status, string? type, string? search, int? agencyId, int? departmentId, int? sectionId, CancellationToken cancellationToken = default);
    Task<ItemDto> GetItemAsync(int id, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<ItemDto> CreateItemAsync(CreateItemDto dto, int currentUserId, CancellationToken cancellationToken = default);
    Task<ItemDto> UpdateItemAsync(int id, CreateItemDto dto, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<ItemDto> CompleteItemAsync(int id, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<ExportFileResult> ExportItemsAsync(int currentUserId, string currentRole, string? status, string? type, string? search, int? agencyId, int? departmentId, int? sectionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ItemAuditDto>> GetAuditLogAsync(int id, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(int id, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<ChatMessageDto> SendMessageAsync(int id, SendMessageDto dto, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<(string PhysicalPath, string DownloadName)> GetAttachmentAsync(int messageId, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task MarkMessagesReadAsync(int id, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<ItemDto> AddMemberAsync(int id, MemberActionDto dto, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
    Task<ItemDto> RemoveMemberAsync(int id, int userId, int currentUserId, string currentRole, CancellationToken cancellationToken = default);
}
