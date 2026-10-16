using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        ///  Register a new Organiser or Participant account.
        /// </summary>  
        /// <param name="dto">Name, email, pasword and role ("Organiser" or "Participant").</param>
        ///  <response code= "200">Account created.l</response>
        ///  <response code= "400">Missing fields, email already registerd, or invalid role.</response>
        /// <returns></returns>

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register(RegisterDto dto)
        {

            //  Check input
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Name, email and password are required!.");
            }

            // Check if email exists
            bool emailTaken = _context.Organisers.Any(o => o.OrganiserEmail == dto.Email) 
                || _context.Participants.Any(p => p.ParticipantEmail == dto.Email);

            if (emailTaken)
                return BadRequest("That email is already registerd");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            

            if (dto.Role == "Organiser")
            { 
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
            return StatusCode(StatusCodes.Status201Created, "Registered");
        }


        /// <summary>
        ///  Logs in an existing user and starts their session. 
        /// </summary>  
        ///  <response code= "200">Login successful</response>
        ///  <response code= "401">Invalid email or password</response>
        /// <param name="dto"></param>
        /// <returns></returns>

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login(LoginDto dto)
        {
            // First, check email to see if its Organiser
            var organiser = _context.Organisers.FirstOrDefault(o => o.OrganiserEmail == dto.Email);
            if (organiser != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, organiser.OrganiserPassword))
                    return Unauthorized("Invalid email or password");

                HttpContext.Session.SetInt32(SessionsKeyUserId, organiser.OrganiserId);
                HttpContext.Session.SetString(SessionsKeyRole, "Organiser");
                return Ok("Login Successfull");
            }

            // Check to see if its Participant
            var participant = _context.Participants.FirstOrDefault(p => p.ParticipantEmail == dto.Email);

            if (participant != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, organiser.OrganiserPassword))
                    return Unauthorized("Invalid email or password");

                HttpContext.Session.SetInt32(SessionsKeyUserId, participant.ParticipantId);
                HttpContext.Session.SetString(SessionsKeyRole, "Participant");
                return Ok("Login Successfull");
            }
            // Email not found in either table
            return Unauthorized("Invalid email or password");
        }

        /// <summary>
        ///  Logs out the current user by clearing their session.
        /// </summary>  
        /// <param name="dto">Email and password</param>
        ///  <response code= "200">Sucessfully logged out.</response>
        /// <returns></returns>
      
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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