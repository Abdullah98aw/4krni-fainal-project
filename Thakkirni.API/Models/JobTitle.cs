using System.ComponentModel.DataAnnotations;

namespace Thakkirni.API.Models
{
    public class JobTitle
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
