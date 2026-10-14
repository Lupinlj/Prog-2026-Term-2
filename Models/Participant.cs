namespace RaceDay.Models
{
    public class Participant
    {
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string ParticipantEmail { get; set; } = string.Empty;
        public string ParticipantPassword { get; set; } = string.Empty;
    }
}
