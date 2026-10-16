using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using RaceDay.Models;
using RaceDay.DTO;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class ProfileController : ControllerBase
    {

        private readonly AppDbContext _context;
        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Returns the logged-in user's name and email
        /// </summary> 
        ///  <response code= "200">Profile returned.</response>
        ///  <response code= "401">Not logged in.</response>
        ///  <response code= "404">User no longer exists.</response>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult GetProfile()
        {
            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);
            var role = HttpContext.Session.GetString(AuthController.SessionsKeyRole);


            if (userId == null)
                return Unauthorized("Please log in");

            if (role == "Organiser")
            {
                var organiser = _context.Organisers.FirstOrDefault(o => o.OrganiserId == userId.Value);

                if (organiser == null)
                    return NotFound("Organiser not found");

                return Ok(new { Name = organiser.OrganiserName, Email = organiser.OrganiserEmail });
            }

            if (role == "Participant")
            {
                var participant = _context.Participants.FirstOrDefault(p => p.ParticipantId == userId.Value);

                if (participant == null)
                    return NotFound("Participant not found");

                return Ok(new { Name = participant.ParticipantName, Email = participant.ParticipantEmail });
            }

            // Role in the session was neither Organiser nor Participant
            return Unauthorized("Please log in");
        }

        /// <summary>
        ///  Updates the logged-in user's name and email.
        /// </summary>  
        /// <param name="dto">New name and email.</param>
        ///  <response code= "200">Profile updated.</response>
        ///  <response code= "400">Invalid fields, or the email is already used by another account.</response>
        ///  <response code= "401">Not logged in.</response>
        ///  <response code= "404">User no longer exists.</response>
        /// <returns></returns>

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdateProfile(ProfileDto dto)
        {


            var userId = HttpContext.Session.GetInt32(AuthController.SessionsKeyUserId);
            var role = HttpContext.Session.GetString(AuthController.SessionsKeyRole);

            if (userId == null)
                return Unauthorized("Please log in");

            if (role == "Organiser")
            {
                
                bool emailTaken = _context.Organisers.Any(o => o.OrganiserEmail == dto.Email && o.OrganiserId != userId.Value) || _context.Participants.Any(p => p.ParticipantEmail == dto.Email);

                if (emailTaken)
                    return BadRequest("That email is already registerd");

                var organiser = _context.Organisers.FirstOrDefault(o => o.OrganiserId == userId.Value);
                if (organiser == null)
                    return NotFound("Organsier not found");

                organiser.OrganiserName = dto.Name;
                organiser.OrganiserEmail = dto.Email;
                _context.SaveChanges();

                return Ok(new { name = organiser.OrganiserName, Email = organiser.OrganiserEmail });
            }

            if (role == "Participant")
            {
                bool emailTaken = _context.Participants.Any(p => p.ParticipantEmail == dto.Email && p.ParticipantId != userId.Value) || _context.Participants.Any(p => p.ParticipantEmail == dto.Email);

                if (emailTaken)
                    return BadRequest("That email is already registerd");

                var participant = _context.Participants.FirstOrDefault(p => p.ParticipantId == userId.Value);
                if (participant == null)
                    return NotFound("Organsier not found");

                participant.ParticipantName = dto.Name;
                participant.ParticipantEmail = dto.Email;
                _context.SaveChanges();

                return Ok(new { name = participant.ParticipantName, Email = participant.ParticipantEmail });
            }

            return Unauthorized("Please log in");
        }



    }

}
