using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Net;

namespace RaceDay.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public ProfileController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        // Show the profile page
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") == null)
                return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("RaceDayApi");

            var cookie = HttpContext.Session.GetString("ApiCookie");
            if (cookie != null)
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync("api/Profile");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var profile = await response.Content.ReadFromJsonAsync<ProfileViewModel>();

            return View(profile);
        }

        // Update the profile
        [HttpPost]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (HttpContext.Session.GetString("Role") == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var client = _factory.CreateClient("RaceDayApi");

            var cookie = HttpContext.Session.GetString("ApiCookie");
            if (cookie != null)
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            // Call the API to update
            var response = await client.PutAsJsonAsync("api/Profile", new
            {
                name = model.Name,
                email = model.Email
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

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }
    }
}