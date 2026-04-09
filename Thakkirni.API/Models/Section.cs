using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thakkirni.API.Models
{
    public class Section
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
<<<<<<< HEAD
        public string? Name { get; set; } = string.Empty;
=======
        public string Name { get; set; }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62

        [Required]
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
