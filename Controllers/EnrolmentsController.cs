using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using RaceDay.DTO;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EnrolmentsController (AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("events/{eventId}/enrolments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult Enrol(int eventId, EnrolmentDto dto)
        {

            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);
            var role = HttpContext.Session.GetString(AuthController.SessionsKeyRole);

            if (role != "Participant")
                return StatusCode(StatusCodes.Status403Forbidden, "Participant only");

            var eventItem = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            if (eventItem == null)
                return NotFound("Event not found");


            // Check if the category exits and belong to this event
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == dto.CategoryId && c.EventId == eventId);

            if (category == null)
                return BadRequest("Invalid category for this event");

            // This will check if the participant is already in any category of the event
            bool alreadyEnrolled = _context.Enrolments.Any(en => en.ParticipantId == userId.Value && _context.Categories.Any(c => c.CategoryId == en.CategoryId && c.EventId == eventId));

            if (alreadyEnrolled)
                return BadRequest("You are already enrolloed in this event");

            // Create and save the enrolment
            var enrolment = new Enrolment
            {

                ParticipantId = userId.Value,
                CategoryId = dto.CategoryId
            };

            _context.Enrolments.Add(enrolment);
            _context.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new { enrolment.EnrolmentId, enrolment.Category });
        }


        [HttpGet("enrolments/mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetMyEnrolments()
        {
            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);
            var role = HttpContext.Session.GetString(AuthController.SessionsKeyRole);


            if (userId == null)
                return Unauthorized("Please log in");

            if (role != "Participant")
                return StatusCode(StatusCodes.Status403Forbidden, "Participat only");

            // Get this event's enrolments, with each participant's details
            var enrolments = (from en in _context.Enrolments
                              join c in _context.Categories on en.CategoryId equals c.CategoryId
                              join e in _context.Events on c.EventId equals e.EventId
                              where en.ParticipantId == userId.Value
                              select new
                              {
                                  en.EnrolmentId,
                                  c.CategoryId,
                                  c.CategoryName,
                                  e.EventId,
                                  e.EventName,
                                  e.EventDate
                              }).ToList();


            return Ok(enrolments);
        }
        [HttpGet("events/{eventId}/enrolments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult GetMyEnrolments(int eventId)
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


            // Organiser can only view enrolments for their own events
            if (eventItem.OrganiserId != userId.Value)
                return StatusCode(StatusCodes.Status403Forbidden, "You can only view enrolmets for your own events");

            // Get this event's enrolments, with each participant's details
            var enrolments = (from en in _context.Enrolments
                              join c in _context.Categories on en.CategoryId equals c.CategoryId
                              join e in _context.Events on c.EventId equals e.EventId
                              where en.ParticipantId == userId.Value
                              select new
                              {
                                  en.EnrolmentId,
                                  c.CategoryId,
                                  c.CategoryName,
                                  e.EventId,
                                  e.EventName,
                                  e.EventDate
                              }).ToList();
            return Ok(enrolments);
        }



    }
}
