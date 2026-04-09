using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thakkirni.API.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string Text { get; set; }

        public string PdfAttachmentFileName { get; set; }
        public string PdfAttachmentPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
