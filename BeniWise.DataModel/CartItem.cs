using System.ComponentModel.DataAnnotations;

namespace BeniWise.DataModel
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // Which cart this item belongs to
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        // Which menu item was added
        public int MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        // Quantity ordered
        public int Quantity { get; set; }
    }
}