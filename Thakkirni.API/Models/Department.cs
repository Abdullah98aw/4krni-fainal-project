using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thakkirni.API.Models
{
    public class Department
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
        public int AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency Agency { get; set; }

        public ICollection<Section> Sections { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
