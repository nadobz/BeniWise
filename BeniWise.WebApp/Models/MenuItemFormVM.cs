using System.ComponentModel.DataAnnotations;

namespace BeniWise.WebApp.Models
{
    public class MenuItemFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a menu item name.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a description.")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please enter a price.")]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please enter the calorie count.")]
        [Range(0, 5000, ErrorMessage = "Calories must be between 0 and 5000.")]
        public int Calories { get; set; }

        [Required(ErrorMessage = "Please enter the ingredients.")]
        public string? Ingredients { get; set; }

        [Required(ErrorMessage = "Please enter the allergens.")]
        public string? Allergens { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        // Optional here because Edit can keep the existing photo.
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImagePath { get; set; }
    }
}