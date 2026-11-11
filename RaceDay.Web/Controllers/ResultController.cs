using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Net;

namespace RaceDay.Web.Controllers
{
    public class ResultsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public ResultsController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        // Show the form to enter a result
        [HttpGet]
        public IActionResult Create(int enrolmentId, int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var model = new ResultFormViewModel
            {
                EnrolmentId = enrolmentId,
                EventId = eventId
            };

            return View(model);
        }

        // Save the result to the API
        [HttpPost]
        public async Task<IActionResult> Create(ResultFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            // Call your API endpoint: POST /api/enrolments/{enrolmentId}/result
            var response = await client.PostAsJsonAsync($"api/enrolments/{model.EnrolmentId}/result", new
            {
                finishTime = model.FinishTime,
                position = model.Position
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

            TempData["Success"] = "Result saved successfully!";

            // Redirect back to the Organiser's Enrolments page for this event
            return RedirectToAction("OrganiserIndex", "Enrolments", new { eventId = model.EventId });
        }
    }
}