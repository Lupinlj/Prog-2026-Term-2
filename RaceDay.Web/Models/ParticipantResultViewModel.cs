namespace RaceDay.Web.Models
{
    public class ParticipantResultViewModel
    {
        public int ResultID { get; set; }
        public TimeOnly FinishTime { get; set; }
        public int Position { get; set; }
        public string EventName { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = "";
    }
}