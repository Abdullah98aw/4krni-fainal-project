using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thakkirni.API.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string ItemNumber { get; set; } // e.g., T-10001

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } // "TASK" or "COMMITTEE"

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string Importance { get; set; } // "NORMAL" or "SECRET"

        [MaxLength(20)]
        public string? CommitteeType { get; set; } // "INTERNAL" or "EXTERNAL"

        // Status is NO LONGER stored — it is computed dynamically:
        //   CompletedDate != null  => "COMPLETED"
        //   DueDate < UtcNow       => "OVERDUE"
        //   otherwise              => "ACTIVE"
        public DateTime? CompletedDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; }

        [Required]
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ItemMember> Members { get; set; }
        public ICollection<ItemAssignee> Assignees { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
        public ICollection<AuditEvent> AuditEvents { get; set; }

        // ── Computed status helper (used in backend only) ──────────────────
        [NotMapped]
        public string ComputedStatus =>
            CompletedDate.HasValue ? "COMPLETED" :
            DueDate < DateTime.UtcNow ? "OVERDUE" :
            "ACTIVE";
    }
}
