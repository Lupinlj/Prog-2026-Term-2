using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class ResultFormViewModel
    {
        [Required]
        public int EnrolmentId { get; set; }

        [Required]
        public TimeOnly FinishTime { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Position { get; set; }
        public int EventId { get; set; }
    }
}