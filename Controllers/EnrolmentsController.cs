using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Net;

namespace RaceDay.Web.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public EnrolmentsController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<IActionResult> Index(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            var response = await client.GetAsync($"api/events/{eventId}/enrolments");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Index", "Events");
            }

            var list = await response.Content.ReadFromJsonAsync<List<EnrolmentViewModel>>();

            return View(new EnrolmentsPageViewModel
            {
                EventId = eventId,
                Enrolments = list!
            });
        }
    }
}