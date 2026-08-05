using System.ComponentModel.DataAnnotations;

namespace BeniWise.WebApp.Models
{
    public class MenuItemFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 10000)]
        public decimal Price { get; set; }

        [Range(0, 5000)]
        public int Calories { get; set; }

        public string? Ingredients { get; set; }
        public string? Allergens { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? ExistingImagePath { get; set; } // used on Edit
    }
}