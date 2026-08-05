using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeniWise.DataModel
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }

        public int Calories { get; set; }

        // Comma-separated for now, e.g. "Chicken, Soy Sauce, Garlic, Vinegar"
        public string? Ingredients { get; set; }

        // Comma-separated for now, e.g. "Soy, Dairy"
        public string? Allergens { get; set; }

        // Relative path under wwwroot, e.g. "/uploads/menu/abc123.jpg"
        public string? ImagePath { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
