using System;
using System.Collections.Generic;
<<<<<<< HEAD
using Microsoft.AspNetCore.Http;
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62

namespace Thakkirni.API.DTOs
{
    public class ItemDto
    {
        public int Id { get; set; }
<<<<<<< HEAD
        public string ItemNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Importance { get; set; } = string.Empty;
        public string? CommitteeType { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletedDate { get; set; }
        public DateTime DueDate { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int? AgencyId { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<int> MemberIds { get; set; } = new();
        public List<int> AssigneeIds { get; set; } = new();
        public List<UserDto> Members { get; set; } = new();
        public List<UserDto> Assignees { get; set; } = new();
        public bool HasUnreadUpdates { get; set; }
    }

    public class ItemAuditDto
    {
        public int Id { get; set; }
        public string ActionType { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
=======
        public string ItemNumber { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Importance { get; set; }
        public string? CommitteeType { get; set; }

        // Computed dynamically — never stored in DB
        public string Status { get; set; }

        public DateTime? CompletedDate { get; set; }
        public DateTime DueDate { get; set; }
        public int CreatedById { get; set; }
        public int DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<int> MemberIds { get; set; }
        public List<int> AssigneeIds { get; set; }
        public int UnreadCount { get; set; }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    }

    public class SendMessageDto
    {
<<<<<<< HEAD
        public string? Text { get; set; }
        public IFormFile? Attachment { get; set; }
    }

    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public string? AttachmentFileName { get; set; }
        public bool HasAttachment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
=======
        public string Text { get; set; }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    }

    public class CreateItemDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Importance { get; set; }
        public string? CommitteeType { get; set; }
        public DateTime DueDate { get; set; }
        public List<int>? MemberIds { get; set; }
        public List<int>? AssigneeIds { get; set; }
<<<<<<< HEAD
        public List<string>? MemberNationalIds { get; set; }
        public List<string>? AssigneeNationalIds { get; set; }
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    }

    public class MemberActionDto
    {
        public int UserId { get; set; }
    }
}
