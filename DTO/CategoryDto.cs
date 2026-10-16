using System.ComponentModel.DataAnnotations;

namespace RaceDay.DTO
{
    public class CategoryDto
    {

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;
    }
}
