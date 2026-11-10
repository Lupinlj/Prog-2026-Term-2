using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Diagnostics;
using System.Net;

namespace RaceDay.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public HomeController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> OrganiserHome()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser") return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");
            var cookie = HttpContext.Session.GetString("ApiCookie");
            if (cookie != null) client.DefaultRequestHeaders.Add("Cookie", cookie);

            // Get all events
            var response = await client.GetAsync("Api/Events");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var allEvents = await response.Content.ReadFromJsonAsync<List<EventViewModel>>();
            var myId = HttpContext.Session.GetInt32("UserId");

            // Keep only this organiser's events
            var myEvents = allEvents!.Where(e => e.OrganiserId == myId).ToList();

            return View(myEvents);

        }

        public async Task<IActionResult> ParticipantHome(string searchTerm, string eventTypeFilter)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant") return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");
            var cookie = HttpContext.Session.GetString("ApiCookie");
            if (cookie != null) client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync("Api/Events");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var allEvents = await response.Content.ReadFromJsonAsync<List<EventViewModel>>();
            var filteredEvents = allEvents ?? new List<EventViewModel>();

            // 1. Only show UPCOMING events (today or in the future)
            filteredEvents = filteredEvents.Where(e => e.EventDate >= DateTime.Today).ToList();

            // 2. Filter by Search Term (Event Name or Location)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredEvents = filteredEvents.Where(e =>
                    e.EventName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.EventLocation.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // 3. Filter by Event Type
            if (!string.IsNullOrEmpty(eventTypeFilter))
            {
                filteredEvents = filteredEvents.Where(e => e.EventType == eventTypeFilter).ToList();
            }

            // Pass the data to the view
            var viewModel = new ParticipantHomeViewModel
            {
                SearchTerm = searchTerm,
                EventTypeFilter = eventTypeFilter,
                Events = filteredEvents
            };

            return View(viewModel);
        }
    }
}
