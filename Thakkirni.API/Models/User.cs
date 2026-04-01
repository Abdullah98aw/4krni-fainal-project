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
        public string Name { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string NationalId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } // "USER" or "ADMIN"

        public string Avatar { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public ICollection<ItemMember> ItemMembers { get; set; }
        public ICollection<ItemAssignee> ItemAssignees { get; set; }
    }
}
