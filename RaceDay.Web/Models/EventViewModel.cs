namespace RaceDay.Web.Models
{
    public class EventViewModel
    {

        public int EventId { get; set;  }
        public string EventName { get; set; } = "";
        public string EventDescription { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; } = "";
        public decimal Distance { get; set; } 
        public string EventType { get; set; } = "";
        public int OrganiserId { get; set; }

        public string ImageUrl { get; set; } = "";
    }
}
