using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class ProfileViewModel
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}