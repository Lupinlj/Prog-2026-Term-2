namespace RaceDay.Web.Models
{
    public class EnrolmentsPageViewModel
    {
        public int EventId { get; set; }
        public List<EnrolmentViewModel> Enrolments { get; set; } = new();
    }
}