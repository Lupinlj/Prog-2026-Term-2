namespace RaceDay.Web.Models
{
    public class EventDetailsViewModel
    {
        public EventViewModel Event { get; set; } = new EventViewModel();
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        // This is what the user will select from a dropdown
        public int SelectedCategoryId { get; set; }
    }
}