namespace RaceDay.Models
{
    public class Result
    {

        public int ResultId { get; set; }
        public TimeOnly FinishTime { get; set; }
        public int Position { get; set; }
        public int EnrolmentId { get; set; }
        public Enrolment Enrolment { get; set; }
    }
}
