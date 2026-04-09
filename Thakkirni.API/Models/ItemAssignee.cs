using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thakkirni.API.Models
{
    public class ItemAssignee
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
