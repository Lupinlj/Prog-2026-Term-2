using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{
    public class Category
    {

        public int CategoryId {  get; set; } 
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;
       
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}
