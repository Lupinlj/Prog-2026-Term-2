using Microsoft.AspNetCore.Mvc;
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
        

        
    }
}
