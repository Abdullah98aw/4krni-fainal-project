using System.ComponentModel.DataAnnotations;

namespace Thakkirni.API.Models
{
    public class Agency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; } = string.Empty;

        public ICollection<Department> Departments { get; set; }
    }
}
