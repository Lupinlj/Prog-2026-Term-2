using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RaceDay.Web.Models;
using System.Net;

namespace RaceDay.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public EventsController(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            var response = await client.GetAsync("Api/Events");

            // API session expired: log out and start again
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var events = await response.Content.ReadFromJsonAsync<List<EventViewModel>>();

            // The API returns every event, so keep only this Organiser's own
            var myId = HttpContext.Session.GetInt32("UserId");
            var mine = events!.Where(e => e.OrganiserId == myId).ToList();

            return View(mine);

        }


        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            return View(new EventsFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> create (EventsFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            var response = await client.PostAsJsonAsync("api/Events", new
            {
                eventName = model.EventName,
                eventDescription = model.EventDescription,
                eventDate = model.EventDate,
                eventLocation = model.EventLocation,
                distance = model.Distance,
                eventType = model.EventType
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
