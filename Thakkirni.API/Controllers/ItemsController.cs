using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Thakkirni.API.Data;
using Thakkirni.API.DTOs;
using Thakkirni.API.Models;

namespace Thakkirni.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var departmentIdStr = User.FindFirst("DepartmentId")?.Value;
            int? departmentId = string.IsNullOrEmpty(departmentIdStr) ? null : int.Parse(departmentIdStr);

            var query = _context.Items
                .Include(i => i.Members)
                .Include(i => i.Assignees)
                .AsQueryable();

            if (userRole == "USER")
            {
                query = query.Where(i => i.CreatedById == userId ||
                                         i.Members.Any(m => m.UserId == userId) ||
                                         i.Assignees.Any(a => a.UserId == userId));
            }
            else if (userRole == "ADMIN" && departmentId.HasValue)
            {
                query = query.Where(i => i.DepartmentId == departmentId.Value);
            }

            var items = await query.ToListAsync();

            // Get unread counts
            var unreadCounts = await _context.ChatMessages
                .Where(m => !_context.MessageReadStatuses.Any(r => r.MessageId == m.Id && r.UserId == userId))
                .GroupBy(m => m.ItemId)
                .Select(g => new { ItemId = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(items.Select(i => new ItemDto
            {
                Id = i.Id,
                ItemNumber = i.ItemNumber,
                Type = i.Type,
                Title = i.Title,
                Description = i.Description,
                Importance = i.Importance,
                CommitteeType = i.CommitteeType,
                Status = i.Status,
                DueDate = i.DueDate,
                CreatedById = i.CreatedById,
                DepartmentId = i.DepartmentId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                MemberIds = i.Members.Select(m => m.UserId).ToList(),
                AssigneeIds = i.Assignees.Select(a => a.UserId).ToList(),
                UnreadCount = unreadCounts.FirstOrDefault(u => u.ItemId == i.Id)?.Count ?? 0
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var item = await _context.Items
                .Include(i => i.Members)
                .Include(i => i.Assignees)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            return Ok(new ItemDto
            {
                Id = item.Id,
                ItemNumber = item.ItemNumber,
                Type = item.Type,
                Title = item.Title,
                Description = item.Description,
                Importance = item.Importance,
                CommitteeType = item.CommitteeType,
                Status = item.Status,
                DueDate = item.DueDate,
                CreatedById = item.CreatedById,
                DepartmentId = item.DepartmentId,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                MemberIds = item.Members.Select(m => m.UserId).ToList(),
                AssigneeIds = item.Assignees.Select(a => a.UserId).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto createDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var departmentIdStr = User.FindFirst("DepartmentId")?.Value;

            if (string.IsNullOrEmpty(departmentIdStr))
                return BadRequest("User does not belong to a department.");

            var departmentId = int.Parse(departmentIdStr);

            var item = new Item
            {
                ItemNumber = (createDto.Type == "TASK" ? "T-" : "C-") + new Random().Next(10000, 99999),
                Type = createDto.Type,
                Title = createDto.Title,
                Description = createDto.Description ?? "",
                Importance = createDto.Importance,
                CommitteeType = createDto.CommitteeType,
                Status = "TODO",
                DueDate = createDto.DueDate,
                CreatedById = userId,
                DepartmentId = departmentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Members = createDto.MemberIds?.Select(id => new ItemMember { UserId = id }).ToList() ?? new List<ItemMember>(),
                Assignees = createDto.AssigneeIds?.Select(id => new ItemAssignee { UserId = id }).ToList() ?? new List<ItemAssignee>()
            };

            if (!item.Members.Any(m => m.UserId == userId))
                item.Members.Add(new ItemMember { UserId = userId });

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            _context.AuditEvents.Add(new AuditEvent
            {
                ItemId = item.Id,
                Type = "CREATE",
                UserId = userId,
                MetaData = $"Created: {item.Title}",
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, new ItemDto
            {
                Id = item.Id,
                ItemNumber = item.ItemNumber,
                Type = item.Type,
                Title = item.Title,
                Description = item.Description,
                Importance = item.Importance,
                CommitteeType = item.CommitteeType,
                Status = item.Status,
                DueDate = item.DueDate,
                CreatedById = item.CreatedById,
                DepartmentId = item.DepartmentId,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                MemberIds = item.Members.Select(m => m.UserId).ToList(),
                AssigneeIds = item.Assignees.Select(a => a.UserId).ToList()
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] CreateItemDto updateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var item = await _context.Items
                .Include(i => i.Members)
                .Include(i => i.Assignees)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            // Update scalar fields
            item.Title = updateDto.Title ?? item.Title;
            item.Description = updateDto.Description ?? item.Description;
            item.Importance = updateDto.Importance ?? item.Importance;
            item.CommitteeType = updateDto.CommitteeType ?? item.CommitteeType;
            item.DueDate = updateDto.DueDate != default ? updateDto.DueDate : item.DueDate;
            item.UpdatedAt = DateTime.UtcNow;

            // Re-sync Members if provided
            if (updateDto.MemberIds != null)
            {
                _context.ItemMembers.RemoveRange(item.Members);
                item.Members = updateDto.MemberIds
                    .Select(uid => new ItemMember { ItemId = id, UserId = uid })
                    .ToList();
                // Ensure the creator is always a member
                if (!item.Members.Any(m => m.UserId == item.CreatedById))
                    item.Members.Add(new ItemMember { ItemId = id, UserId = item.CreatedById });
            }

            // Re-sync Assignees if provided
            if (updateDto.AssigneeIds != null)
            {
                _context.ItemAssignees.RemoveRange(item.Assignees);
                item.Assignees = updateDto.AssigneeIds
                    .Select(uid => new ItemAssignee { ItemId = id, UserId = uid })
                    .ToList();
            }

            // Write audit event
            _context.AuditEvents.Add(new AuditEvent
            {
                ItemId = id,
                Type = "UPDATE",
                UserId = userId,
                MetaData = $"Updated: {item.Title}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new ItemDto
            {
                Id = item.Id,
                ItemNumber = item.ItemNumber,
                Type = item.Type,
                Title = item.Title,
                Description = item.Description,
                Importance = item.Importance,
                CommitteeType = item.CommitteeType,
                Status = item.Status,
                DueDate = item.DueDate,
                CreatedById = item.CreatedById,
                DepartmentId = item.DepartmentId,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                MemberIds = item.Members.Select(m => m.UserId).ToList(),
                AssigneeIds = item.Assignees.Select(a => a.UserId).ToList()
            });
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteItem(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var item = await _context.Items
                .Include(i => i.Members)
                .Include(i => i.Assignees)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            item.Status = "COMPLETED";
            item.UpdatedAt = DateTime.UtcNow;

            _context.AuditEvents.Add(new AuditEvent
            {
                ItemId = item.Id,
                Type = "COMPLETE",
                UserId = userId,
                MetaData = $"Completed: {item.Title}",
                CreatedAt = DateTime.UtcNow
            });

            // Notify members
            foreach (var member in item.Members)
            {
                if (member.UserId != userId)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = member.UserId,
                        Title = "تم إتمام المهمة",
                        Body = $"تم إتمام: {item.Title}",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new ItemDto
            {
                Id = item.Id,
                ItemNumber = item.ItemNumber,
                Type = item.Type,
                Title = item.Title,
                Description = item.Description,
                Importance = item.Importance,
                CommitteeType = item.CommitteeType,
                Status = item.Status,
                DueDate = item.DueDate,
                CreatedById = item.CreatedById,
                DepartmentId = item.DepartmentId,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                MemberIds = item.Members.Select(m => m.UserId).ToList(),
                AssigneeIds = item.Assignees.Select(a => a.UserId).ToList()
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Messages
        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetMessages(int id)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.ItemId == id)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new {
                    m.Id,
                    m.ItemId,
                    m.UserId,
                    m.Text,
                    m.PdfAttachmentFileName,
                    m.PdfAttachmentPath,
                    m.CreatedAt,
                    UserName = _context.Users.Where(u => u.Id == m.UserId).Select(u => u.Name).FirstOrDefault()
                })
                .ToListAsync();
            return Ok(messages);
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> SendMessage(int id, [FromBody] SendMessageDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var item = await _context.Items.Include(i => i.Members).FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return NotFound();

            var message = new ChatMessage
            {
                ItemId = id,
                UserId = userId,
                Text = dto.Text,
                PdfAttachmentFileName = "",
                PdfAttachmentPath = "",
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Notify members
            foreach (var member in item.Members)
            {
                if (member.UserId != userId)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = member.UserId,
                        Title = "رسالة جديدة",
                        Body = $"رسالة جديدة في: {item.Title}",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            var userName = await _context.Users.Where(u => u.Id == userId).Select(u => u.Name).FirstOrDefaultAsync();

            return Ok(new {
                message.Id,
                message.ItemId,
                message.UserId,
                message.Text,
                message.CreatedAt,
                UserName = userName
            });
        }

        [HttpPost("{id}/messages/read")]
        public async Task<IActionResult> MarkMessagesRead(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var messages = await _context.ChatMessages.Where(m => m.ItemId == id).ToListAsync();

            foreach (var msg in messages)
            {
                if (!await _context.MessageReadStatuses.AnyAsync(r => r.MessageId == msg.Id && r.UserId == userId))
                {
                    _context.MessageReadStatuses.Add(new MessageReadStatus { MessageId = msg.Id, UserId = userId });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ── Member Management ──────────────────────────────────────────────────

        /// <summary>Add a member to a task by UserId.</summary>
        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMember(int id, [FromBody] MemberActionDto dto)
        {
            var callerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var item = await _context.Items
                .Include(i => i.Members)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound(new { message = "المهمة غير موجودة" });

            // Verify the target user exists
            var targetUser = await _context.Users.FindAsync(dto.UserId);
            if (targetUser == null) return NotFound(new { message = "المستخدم غير موجود" });

            // Prevent duplicate membership
            if (item.Members.Any(m => m.UserId == dto.UserId))
                return Conflict(new { message = "المستخدم عضو بالفعل" });

            _context.ItemMembers.Add(new ItemMember { ItemId = id, UserId = dto.UserId });

            _context.AuditEvents.Add(new AuditEvent
            {
                ItemId = id,
                Type = "ADD_MEMBER",
                UserId = callerId,
                MetaData = $"Added member: {targetUser.Name}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم إضافة العضو بنجاح" });
        }

        /// <summary>Remove a member from a task by UserId.</summary>
        [HttpDelete("{id}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int id, int userId)
        {
            var callerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var item = await _context.Items
                .Include(i => i.Members)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound(new { message = "المهمة غير موجودة" });

            // Prevent removing the creator
            if (item.CreatedById == userId)
                return BadRequest(new { message = "لا يمكن إزالة منشئ المهمة" });

            var membership = item.Members.FirstOrDefault(m => m.UserId == userId);
            if (membership == null) return NotFound(new { message = "المستخدم ليس عضواً" });

            _context.ItemMembers.Remove(membership);

            var removedUser = await _context.Users.FindAsync(userId);
            _context.AuditEvents.Add(new AuditEvent
            {
                ItemId = id,
                Type = "REMOVE_MEMBER",
                UserId = callerId,
                MetaData = $"Removed member: {removedUser?.Name ?? userId.ToString()}",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم إزالة العضو بنجاح" });
        }
    }
}
