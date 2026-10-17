using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Controllers;
using RaceDay.Data;
using Microsoft.EntityFrameworkCore;


namespace RaceDay.Tests
{
    public class EventsTest
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }


        [Fact]
        public void GetEvents_NotloggedIn_ReturnsUnauthorized()
        {
            // Arrange: fresh fake database, controller with an EMPTY session (nobody logged in)
            var context = CreateContext();
            var controller = new EventsController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Session = new TestSessions() } 
            
            };

            // Act: call the endpoint
            var result = controller.GetEvents();

            // Assert: it should refuse with 401
            Assert.IsType<UnauthorizedObjectResult>(result);
        }


        [Fact]
        public void GetEvents_LoggedIn_ReturnsOk()
        {
            // Arrange: a logged-in user in the fake session
            var context = CreateContext();
            var sessions = new TestSessions();
            sessions.SetInt32("UserId", 1);
            sessions.SetString("Role", "Organiser");

            var controller = new EventsController(context);
            controller.ControllerContext = new ControllerContext
            {

                HttpContext = new DefaultHttpContext { Session = sessions }
            };

            // Act
            var result = controller.GetEvents();

            // Assert: a 200 with a list
            Assert.IsType<OkObjectResult>(result);


        }


    }
}
