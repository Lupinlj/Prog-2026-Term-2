using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class CategoriesPageViewModel
    {
        public int EventId { get; set; }
        public List<CategoryViewModel> Categories { get; set; } = new();

        [Required]
        public string NewCategoryName { get; set; } = "";

    }
}
