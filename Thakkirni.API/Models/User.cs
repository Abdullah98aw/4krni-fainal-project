using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thakkirni.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
<<<<<<< HEAD
        public string? Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? NationalId { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string? Role { get; set; } = "USER";

        public string? Avatar { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? JobTitle { get; set; } = string.Empty;

        [MaxLength(512)]
        public string? PasswordHash { get; set; } = string.Empty;

        public int? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public int? SectionId { get; set; }
        [ForeignKey("SectionId")]
        public Section? Section { get; set; }

        public ICollection<ItemMember> ItemMembers { get; set; } = new List<ItemMember>();
        public ICollection<ItemAssignee> ItemAssignees { get; set; } = new List<ItemAssignee>();
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
=======
        public string Name { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string NationalId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } // "USER" or "ADMIN"

        public string Avatar { get; set; }

        [MaxLength(100)]
        public string JobTitle { get; set; }

        public int? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency Agency { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public int? SectionId { get; set; }
        [ForeignKey("SectionId")]
        public Section Section { get; set; }

        public ICollection<ItemMember> ItemMembers { get; set; }
        public ICollection<ItemAssignee> ItemAssignees { get; set; }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    }
}
