using RaceDay.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTO
{
    public class EventDto
    {
        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string EventDescription { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventLocation { get; set; } = string.Empty;

        [Range(0.01, 1000)]
        public decimal Distance { get; set; }

        [Required]
        public string EventType { get; set; } = string.Empty;
    }
}
