using RaceDay.Models;
using System.ComponentModel.DataAnnotations;


namespace RaceDay.DTO
{
    public class ProfileDto{
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

    }

     

}
