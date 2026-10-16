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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult CreateCategories(int eventId, CategoryDto dto)
        {

            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);
            var role = HttpContext.Session.GetString(AuthController.SessionsKeyRole);

            if (userId == null)
                return Unauthorized("Please log in");

            if (role != "Organiser")
                return StatusCode(StatusCodes.Status403Forbidden, "Organiser only");

            var eventItem = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            if (eventItem == null)
                return NotFound("Event not found");

            // The event must belong to the logged-in organiser
            if (eventItem.OrganiserId != userId.Value)
                return StatusCode(StatusCodes.Status403Forbidden, "You can only add categories to your own events");

            var category = new Category
            {
                CategoryName = dto.CategoryName,
                EventId = eventId
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new { category.CategoryId, category.CategoryName, category.EventId });

        }
    }
}

   


