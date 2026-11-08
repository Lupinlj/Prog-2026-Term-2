using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class EventsFormViewModel
    {

        [Required]
        public string EventName { get; set; } = "";

        [Required]
        public string EventDescription { get; set; } = "";

        [Required]
        public DateTime EventDate { get; set; } = DateTime.Today.AddDays(7);

        [Required]
        public string EventLocation { get; set; } = "";

        [Range(0.01, 1000)]
        public decimal Distance { get; set;  }

        [Required]
        public string EventType { get; set; } = "";

        public int EventId { get; set;  }
    }
}
