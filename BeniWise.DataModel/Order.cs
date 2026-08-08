using System.ComponentModel.DataAnnotations;

namespace BeniWise.DataModel
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        // One order contains many order items
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}