using System.ComponentModel.DataAnnotations;

namespace BeniWise.DataModel
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        // Parent order
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        // Ordered menu item
        public int MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        // Quantity ordered
        public int Quantity { get; set; }

        // Price at the time of purchase
        public decimal Price { get; set; }
    }
}