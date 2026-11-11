using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public AccountController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login (LoginViewModel model)
        {
            var client = _factory.CreateClient("RaceDayApi");

            // Send the login to the api
            var response = await client.PostAsJsonAsync("api/Auth/login", new
            {
                name = "",
                email = model.Email,
                password = model.Password
            });

            // Handle a failed login
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Save the API's session cookie so later calls can show it
            var cookie = response.Headers.GetValues("Set-Cookie").First();
            HttpContext.Session.SetString("ApiCookie", cookie.Split(';')[0]);

            // Read the role from the api's reply
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            HttpContext.Session.SetString("Role", result!.Role);
            HttpContext.Session.SetInt32("UserId", result.UserId);
  
            if (result.Role == "Organiser")
            {
                return RedirectToAction("OrganiserHome", "Home");
            }
            return RedirectToAction("ParticipantHome", "Home");
         
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register (RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);

            }
            
            var client = _factory.CreateClient("RaceDayApi");

            // Send the register details to the api
            var response = await client.PostAsJsonAsync("api/Auth/register", new
            {
                name = model.Name,
                email = model.Email,
                password = model.Password,
                role = model.Role
            });

            // Handle a failed register
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View(model);
            }

            // Registered, go to login
            return RedirectToAction("Login");
            
        }

        public IActionResult Logout()
        {
            // Clear everyting stored in the session (the user ID and role), so they are logged out
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }


    }
}
