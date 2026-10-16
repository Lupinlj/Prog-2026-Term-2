using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using RaceDay.DTO;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/events/{eventId}/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult GetCategories(int eventId)
        {
            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);

            if (userId == null)
                return Unauthorized("Please log in");

            var eventItem = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            if (eventItem == null)
                return NotFound("Event not found");

            var categories = _context.Categories
                .Where(C => C.EventId == eventId)
                .Select(c => new { c.CategoryId, c.CategoryName, c.EventId })
                .ToList();

            return Ok(categories);
        }
    }
}
