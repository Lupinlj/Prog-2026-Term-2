using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Net;

namespace RaceDay.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public CategoriesController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            var response = await client.GetAsync($"api/events/{eventId}/categories");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Events");

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryViewModel>>();

            return View(new CategoriesPageViewModel
            {
                EventId = eventId,
                Categories = categories!
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(int eventId, string newCategoryName)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(newCategoryName))
            {
                TempData["Error"] = "Category name is required.";
                return RedirectToAction("Index", new { eventId });
            }

            var client = _factory.CreateClient("RaceDayApi");
            client.DefaultRequestHeaders.Add("Cookie", HttpContext.Session.GetString("ApiCookie"));

            var response = await client.PostAsJsonAsync($"api/events/{eventId}/categories", new
            {
                categoryName = newCategoryName
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = await response.Content.ReadAsStringAsync();

            return RedirectToAction("Index", new { eventId });
        }
    }
}
