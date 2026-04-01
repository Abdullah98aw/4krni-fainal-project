using System.ComponentModel.DataAnnotations;

namespace Thakkirni.API.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<Department> Departments { get; set; }
    }
}
