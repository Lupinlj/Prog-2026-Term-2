using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;


        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

    }
}
