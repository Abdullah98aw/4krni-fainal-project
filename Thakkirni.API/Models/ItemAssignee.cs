using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thakkirni.API.Models
{
    public class ItemAssignee
    {
<<<<<<< HEAD
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int UserId { get; set; }
=======
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
        public User User { get; set; }
    }
}
