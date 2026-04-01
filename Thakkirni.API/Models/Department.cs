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
        public string Name { get; set; }

        [Required]
        public int AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency Agency { get; set; }

        public ICollection<Section> Sections { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
