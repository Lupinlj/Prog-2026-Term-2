using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTO;
using RaceDay.Models;
using System.ComponentModel;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        private static bool ValidEventType(string type) =>
            type == "Run" || type == "Walk" || type == "Cycle";

        /// <summary>
        ///  Lists all events. Available to any logged-in user.
        /// </summary> 
        ///  <response code= "200">List of events returned.</response>
        ///  <response code= "401">Not logged in.</response>
        /// <returns></returns>
        /// 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetEvents()
        {
            // Read who is logged in from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            // Nobody logged in, the user will get a 401 error
            if (userId == null)
                return Unauthorized("Please log in");

            // Get every event from the database 
            var events = _context.Events.ToList();

            return Ok(events);


        }
        /// <summary>
        ///  Returns a single event by its id
        /// </summary> 
        /// <param name="id">The id of the event.</param>
        ///  <response code= "200">Event returned.</response>
        ///  <response code= "401">Not logged in.</response>
        ///  <response code= "404">Event not found.</response>
        /// <returns></returns>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEvent(int id)
        {

            // Read who is logged in from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            // Nobody logged in, the user will get a 401 error
            if (userId == null)
                return Unauthorized("Please log in");

            // Find the one event with this ID
            var eventItem = _context.Events.FirstOrDefault(e => e.EventId == id);

            // No event qwith the ID, so return 404
            if (eventItem == null)
                return NotFound("Event not found");


            return Ok(eventItem);
        }


        [HttpPost]
        public IActionResult CreateEvent(EventDto dto)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");


            if (userId == null)
                return Unauthorized("Please log in");

            if (role != "Organiser")
                return StatusCode(StatusCodes.Status403Forbidden, "Organiser Only");

            if (string.IsNullOrWhiteSpace(dto.EventName) ||
                string.IsNullOrWhiteSpace(dto.EventLocation))
            {
                return BadRequest("Event name and location are required.");
            }

            if (dto.EventType != "Run" && dto.EventType != "Walk" && dto.EventType != "Cycle")
                return BadRequest("Event type must be Run, Walk or Cycle.");

            // Make a new event using the details the Organiser sent
            var newEvent = new Event
            {
                EventName = dto.EventName,
                EventDescription = dto.EventDescription,
                EventDate = dto.EventDate,
                EventLocation = dto.EventLocation,
                Distance = dto.Distance,
                EventType = dto.EventType,
                OrganiserId = userId.Value
            };


            // Add it and write it to the database
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return Ok(newEvent);
        }

        /// <summary>
        ///  Updates one of the Organiser's own events.
        /// </summary> 
        /// <param name="id">The id of the event to update.</param>
        /// <param name="dto">New name, description, date, location, distance and event type.</param>
        ///  <response code= "200">Event updated.</response>
        ///  <response code= "400">Invalid eent type (must be Run, Walk or Cycle(.</response> 
        ///  <response code= "401">Not logged in.</response>
        ///  <response code= "403">Only Organisers can update events, and only their own.</response> 
        ///  <response code= "404">Event not found.</response>
        /// <returns></returns>
        [HttpPut("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdateEvent(int id, EventDto dto)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");


            if (userId == null)
                return Unauthorized("Please log in");

            if (role != "Organiser")
                return StatusCode(StatusCodes.Status403Forbidden, "Organiser only");


            if (!ValidEventType(dto.EventType))
                return BadRequest("Event type must be Run, Walk or Cycle.");

            var eventItem = _context.Events.FirstOrDefault(e => e.EventId == id);
            if (eventItem == null)
                return NotFound("Event not found");

            // Organisers can only change their own events
            if (eventItem.OrganiserId != userId.Value)
                return StatusCode(StatusCodes.Status403Forbidden, "You can only edit your own events");

            eventItem.EventName = dto.EventName;
            eventItem.EventDescription = dto.EventDescription;
            eventItem.EventDate = dto.EventDate;
            eventItem.EventLocation = dto.EventLocation;
            eventItem.Distance = dto.Distance;
            eventItem.EventType = dto.EventType;
         
            _context.SaveChanges();
            return Ok(eventItem);
        }

        /// <summary>
        ///  Deletes one of the Organiser's own events.
        /// </summary> 
        /// <param name="id">The id of the evvent to delete.</param>
        ///  <response code= "200">Event deleted.</response>
        ///  <response code= "401">Not logged in.</response>
        ///  <response code= "403">Only Organisers can delete events, and only their own..</response> 
        ///  <response code= "404">Event not found.</response>
        /// <returns></returns>

        [HttpDelete("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public IActionResult DeleteEvent(int id)
        {

            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);
            var role = HttpContext.Session.GetString(AuthController.SessionsKeyRole);


            if (userId == null)
                return Unauthorized("Please log in");

            if (role != "Organiser")
                return StatusCode(StatusCodes.Status403Forbidden, "Organiser only");

            var eventItem = _context.Events.FirstOrDefault(e => e.EventId == id);
            if (eventItem == null)
                return NotFound("Event not found");

            if (eventItem.OrganiserId != userId.Value)
                return StatusCode(StatusCodes.Status403Forbidden, "You can only delete your own events");

            var categories = 

            _context.Events.Remove(eventItem);
            _context.SaveChanges();
            return Ok("Event deleted");
        }
    }

}
