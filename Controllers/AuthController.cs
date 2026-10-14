using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Data;
using RaceDay.DTO;
using RaceDay.Models;
using System.Data;

namespace RaceDay.Controllers
{

 
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public const string SessionsKeyUserId = "UserId";
        public const string SessionsKeyRole = "Role";


        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register(RegisterDto dto)
        {

            // 1. Check input
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Name, email and password are required!.");
            }

            // 2. 

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            

            if (dto.Role == "Organiser")
            { 
                // This will check to see if the email isn't already registerd as an Organiser
                if (_context.Organisers.Any(o => o.OrganiserEmail == dto.Email))
                    return BadRequest("That email is already registered");

                // The context the user will use
                _context.Organisers.Add(new Organiser
                {
                    OrganiserName = dto.Name,
                    OrganiserEmail = dto.Email,
                    OrganiserPassword = passwordHash
                });

            }

            else if (dto.Role == "Participant")
            {
                // This will check to see if the email isn't already registerd as an Participant
                if (_context.Participants.Any(p => p.ParticipantEmail == dto.Email))
                    return BadRequest("That email is already registered");


                _context.Participants.Add(new Participant
                {
                    ParticipantName = dto.Name,
                    ParticipantEmail = dto.Email,
                    ParticipantPassword = passwordHash
                });
            }
            else
            {
                // The role can only be Organiser or Participant, reject if neither
                return BadRequest("Role must be Organiser or Participant");
            }
            // Write the new account to the database
            _context.SaveChanges();
            return Ok();
        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login(LoginDto dto)
        {
            // Check the role to see if it's Organiser
            if (dto.Role == "Organiser")
            {
                // Look up the Organiser using the email that was typed in
                var organiser = _context.Organisers.FirstOrDefault(o => o.OrganiserEmail == dto.Email);
               
                // If there is no email, the user will get an error
                if (organiser == null)
                    return Unauthorized("Invalid email or password");

                // Compare the typed password with the stored hash, user gets an error if it doesn't match
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, organiser.OrganiserPassword))
                    return Unauthorized("Invalid email or password");

                // Log user in for Organiser
                HttpContext.Session.SetInt32("UserId", organiser.OrganiserId);
                HttpContext.Session.SetString("Role", "Organiser");
            }
           

            // Check to see of the role is Participant
            else if (dto.Role == "Participant")
            {
                // Look up the Participant using the email that was typed in
                var participant = _context.Participants.FirstOrDefault(p =>p.ParticipantEmail == dto.Email);

                // If there is no email, the user will get an error
                if (participant == null)
                    return Unauthorized("Invalid email or password");

                // Compare the typed password with the stored hash, user gets an error if it doesn't match
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, participant.ParticipantPassword))
                    return Unauthorized("Invalid email or password");

                // Log user in for Participant
                HttpContext.Session.SetInt32("UserId", participant.ParticipantId);
                HttpContext.Session.SetString("Role", "Participant");

            }

            else
            {
                return BadRequest("Role can either be Organiser or Participant");
            }
                return Ok("Login Successful");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        { 
            // Clear everyting stored in the session (the user ID and role), so they are logged out
            HttpContext.Session.Clear();

            return Ok("Logged out"); 
        }

        [HttpPost("status")]
        public IActionResult Status()
        {
            // Read who is logged in from the session
            var id = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            // Nobody logged in, user get error
            if (id == null)
                return Unauthorized("Please log in");

            // Logged in, so report the role and ID
            return Ok($"Logged in as {role}, ID {id}");
        }

        [HttpPost("organiser-only")]
        public IActionResult OrganiserOnly()
        {
            // To check the status of the user is logged in
            var loginCheck = Status();
            if (loginCheck is UnauthorizedObjectResult)
                return loginCheck;

            // Logged in, But not an Organiser, so user gets error
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return StatusCode(StatusCodes.Status403Forbidden, "Organiser only");

            return Ok("Welcome, Organiser");
        }

        [HttpPost("participant-only")]
        public IActionResult ParticipantOnly()
        {
            // To check the status of the user is logged in
            var loginCheck = Status();
            if (loginCheck is UnauthorizedObjectResult)
                return loginCheck;

            // Logged in, But not an Participant, so user gets error
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant")
                return StatusCode(StatusCodes.Status403Forbidden, "Participant only");

            return Ok("Welcome, Participant");
        }
    }
}