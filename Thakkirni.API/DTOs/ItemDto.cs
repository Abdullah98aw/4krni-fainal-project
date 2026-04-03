using System;
using System.Collections.Generic;

namespace Thakkirni.API.DTOs
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Importance { get; set; }
        public string CommitteeType { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public int CreatedById { get; set; }
        public int DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<int> MemberIds { get; set; }
        public List<int> AssigneeIds { get; set; }
        public int UnreadCount { get; set; }
    }

    public class SendMessageDto
    {
        public string Text { get; set; }
    }

    public class CreateItemDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Importance { get; set; }
        public string? CommitteeType { get; set; }
        public DateTime DueDate { get; set; }
        public List<int>? MemberIds { get; set; }
        public List<int>? AssigneeIds { get; set; }
    }

    public class MemberActionDto
    {
        public int UserId { get; set; }
    }
}
