using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using RaceDay.DTO;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api")]
    [ApiController]
    public class ResultController : ControllerBase
    {

        private readonly AppDbContext _context;
        public ResultController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("enrolments/{enrolmentId}/result")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult CaptureResult(int enrolmentId, ResultDto dto)
        {


            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);
            var role = HttpContext.Session.GetString(AuthController.SessionsKeyRole);

            if (userId == null)
                return Unauthorized("Please log in");

            if (role != "Organiser")
                return StatusCode(StatusCodes.Status403Forbidden, "Organiser only");

            var enrolment = _context.Enrolments.FirstOrDefault(en => en.EnrolmentId == enrolmentId);
            if (enrolment == null)
                return NotFound("Enrolment not found");

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == enrolment.CategoryId);
            if (category == null)
                return NotFound("Category not found");

            var eventItem = _context.Events.FirstOrDefault(e => e.EventId == category.EventId);
            if (eventItem == null)
                return NotFound("Event not found");

            // The organiser can only record result for their own events
            if (eventItem.OrganiserId != userId.Value)
                return StatusCode(StatusCodes.Status403Forbidden, "You can only record results for your own events");

            // One result per enrolment

            if (_context.Results.Any(r => r.EnrolmentId == enrolmentId))
                return BadRequest("A result has already been recorded for this enrolment");

            var result = new Result
            {
                EnrolmentId = enrolmentId,
                FinishTime = dto.FinishTime,
                Position = dto.Position
            };

            _context.Results.Add(result);
            _context.SaveChanges();

            return StatusCode(StatusCodes.Status201Created, new { result.ResultId, result.EnrolmentId, result.FinishTime, result.Position });
        }


        [HttpGet("results/mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public IActionResult GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);
            var role = HttpContext.Session.GetString(AuthController.SessionsKeyRole);


            if (userId == null)
                return Unauthorized("Please log in");

            if (role != "Participant")
                return StatusCode(StatusCodes.Status403Forbidden, "Participant only");

            // Get this event's enrolments, with each participant's details
            var results = (from r in _context.Results
                           join en in _context.Enrolments on r.EnrolmentId equals en.EnrolmentId
                           join c in _context.Categories on en.CategoryId equals c.CategoryId
                           join e in _context.Events on c.EventId equals e.EventId
                           where en.ParticipantId == userId.Value
                           select new
                           
                           {
                           
                               r.ResultId,
                               r.FinishTime,
                               r.Position,                                     
                               c.CategoryName,
                               e.EventId,
                               e.EventName,
                               e.EventDate
                              
                           }).ToList();


            return Ok(results);
        }
    }
}
