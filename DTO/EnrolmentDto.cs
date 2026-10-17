using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTO
{
    public class EnrolmentDto
    {
        [Required]
        public int CategoryId { get; set; }
    }
}
