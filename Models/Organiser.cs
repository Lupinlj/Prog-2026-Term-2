using Microsoft.EntityFrameworkCore;
using RaceDay.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{


    [Index(nameof(OrganiserEmail), IsUnique = true)]
    public class Organiser
    {

        public int OrganiserId { get; set; }
        [MaxLength(100)]
        public string OrganiserName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string OrganiserEmail { get; set; } = string.Empty;
        [MaxLength(255)]
        public string OrganiserPassword {  get; set; } = string.Empty;
    }
}
