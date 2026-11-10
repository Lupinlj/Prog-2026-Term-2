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
        public async Task<IActionResult> Create (EventsFormViewModel model)
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


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("cookie", HttpContext.Session.GetString("ApiCookie"));

            var response = await client.GetAsync($"api/Events/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var ev = await response.Content.ReadFromJsonAsync<EventViewModel>();

            // Organiser can only edit their own events
            if (ev!.OrganiserId != HttpContext.Session.GetInt32("UserId"))
                return RedirectToAction("Index");

            return View(new EventsFormViewModel
            {
                EventId = ev.EventId,
                EventName = ev.EventName,
                EventDescription = ev.EventDescription,
                EventDate = ev.EventDate,
                EventLocation = ev.EventLocation,
                Distance = ev.Distance,
                EventType = ev.EventType,

            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventsFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            var response = await client.PutAsJsonAsync($"api/Events/{model.EventId}", new
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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            var response = await client.DeleteAsync($"api/Events/{id}");

            if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = await response.Content.ReadAsStringAsync();


            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            // 1. Get the Event
            var eventResponse = await client.GetAsync($"api/Events/{id}");
            if (!eventResponse.IsSuccessStatusCode) return RedirectToAction("ParticipantHome", "Home");

            var eventData = await eventResponse.Content.ReadFromJsonAsync<EventViewModel>();

            // 2. Get the Categories for this event
            var catResponse = await client.GetAsync($"api/events/{id}/categories");
            var categories = await catResponse.Content.ReadFromJsonAsync<List<CategoryViewModel>>();

            var viewModel = new EventDetailsViewModel
            {
                Event = eventData!,
                Categories = categories ?? new List<CategoryViewModel>()
            };

            return View(viewModel);
        }
    }
}
