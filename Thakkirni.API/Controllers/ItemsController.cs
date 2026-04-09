using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.DTOs;

namespace Thakkirni.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly ICurrentUserService _currentUserService;

    public ItemsController(IItemService itemService, ICurrentUserService currentUserService)
    {
        _itemService = itemService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null, [FromQuery] string? type = null, [FromQuery] string? search = null, [FromQuery] int? agencyId = null, [FromQuery] int? departmentId = null, [FromQuery] int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.GetItemsAsync(currentUser.UserId, currentUser.Role, page, pageSize, status, type, search, agencyId, departmentId, sectionId, cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItem(int id, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.GetItemAsync(id, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> CreateItem([FromBody] CreateItemDto createDto, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        var created = await _itemService.CreateItemAsync(createDto, currentUser.UserId, cancellationToken);
        return CreatedAtAction(nameof(GetItem), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] CreateItemDto updateDto, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.UpdateItemAsync(id, updateDto, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteItem(int id, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.CompleteItemAsync(id, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportItems([FromQuery] string? status = null, [FromQuery] string? type = null, [FromQuery] string? search = null, [FromQuery] int? agencyId = null, [FromQuery] int? departmentId = null, [FromQuery] int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        var export = await _itemService.ExportItemsAsync(currentUser.UserId, currentUser.Role, status, type, search, agencyId, departmentId, sectionId, cancellationToken);
        return File(export.Content, export.ContentType, export.FileName);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteItem(int id) => StatusCode(StatusCodes.Status405MethodNotAllowed, new { message = "حذف المهام غير مسموح في النظام الحالي" });

    [HttpGet("{id}/audit")]
    public async Task<IActionResult> GetAuditLog(int id, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.GetAuditLogAsync(id, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpGet("{id}/messages")]
    public async Task<IActionResult> GetMessages(int id, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.GetMessagesAsync(id, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpPost("{id}/messages")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> SendMessage(int id, [FromForm] SendMessageDto dto, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.SendMessageAsync(id, dto, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpGet("attachments/{messageId}")]
    public async Task<IActionResult> DownloadAttachment(int messageId, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        var file = await _itemService.GetAttachmentAsync(messageId, currentUser.UserId, currentUser.Role, cancellationToken);
        return PhysicalFile(file.PhysicalPath, "application/octet-stream", file.DownloadName);
    }

    [HttpPost("{id}/messages/read")]
    public async Task<IActionResult> MarkMessagesRead(int id, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        await _itemService.MarkMessagesReadAsync(id, currentUser.UserId, currentUser.Role, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddMember(int id, [FromBody] MemberActionDto dto, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.AddMemberAsync(id, dto, currentUser.UserId, currentUser.Role, cancellationToken));
    }

    [HttpDelete("{id}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(int id, int userId, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserService.GetRequiredContext(User);
        return Ok(await _itemService.RemoveMemberAsync(id, userId, currentUser.UserId, currentUser.Role, cancellationToken));
    }
}
