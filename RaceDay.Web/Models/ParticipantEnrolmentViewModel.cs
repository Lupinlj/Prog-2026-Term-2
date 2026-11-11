namespace RaceDay.Web.Models
{
    public class ParticipantEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = "";
    }
}
