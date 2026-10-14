namespace RaceDay.Models
{
    public class Enrolment
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId {  get; set; }
        public int CategoryId { get; set; }
        public Participant Participant { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
