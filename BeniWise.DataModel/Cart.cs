using System.ComponentModel.DataAnnotations;

namespace BeniWise.DataModel
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        // A cart can contain many items
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}