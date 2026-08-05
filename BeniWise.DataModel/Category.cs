using System.ComponentModel.DataAnnotations;

namespace BeniWise.DataModel
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation property — one category has many menu items
        public List<MenuItem> MenuItems { get; set; } = new();
    }
}
