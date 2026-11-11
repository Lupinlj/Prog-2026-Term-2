namespace RaceDay.Web.Models
{
    public class ParticipantHomeViewModel
    {
        // The filtering options
        public string SearchTerm { get; set; } = "";
        public string EventTypeFilter { get; set; } = "";

        // The list of events
        public List<EventViewModel> Events { get; set; } = new List<EventViewModel>();
    }
}
