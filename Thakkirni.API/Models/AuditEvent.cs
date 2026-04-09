using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thakkirni.API.Models
{
    public class AuditEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } // "CREATE", "UPDATE", "DELETE", etc.

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public string MetaData { get; set; } // JSON string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
