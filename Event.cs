namespace RaceDay.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; } = string.Empty;
        public string EventLocation { get; set; } = string.Empty;
        public int OrganiserId { get; set; }

        public decimal Distance { get; set; }

        public string EventType {  get; set; } = string.Empty;
        public Organiser Organisers { get; set; } = null!;
    }
}
