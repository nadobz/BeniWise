using System.ComponentModel.DataAnnotations;

namespace BeniWise.DataModel
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        // Order being reviewed
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        // Customer who submitted the feedback
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime DateSubmitted { get; set; } = DateTime.Now;
    }
}