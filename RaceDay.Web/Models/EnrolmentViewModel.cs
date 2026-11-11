namespace RaceDay.Web.Models
{
    public class EnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = "";
        public string ParticipantEmail { get; set; } = "";
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }
}