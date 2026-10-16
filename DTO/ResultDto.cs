using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTO
{
    public class ResultDto
    {

        [Required]
        public TimeOnly FinishTime { get; set;  }

        [Range(1, 100000)]
        public int Position { get; set; }
    }
}
