using System.ComponentModel.DataAnnotations;

namespace Thakkirni.API.Models
{
    public class Agency
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

        public ICollection<Department> Departments { get; set; }
    }
}
