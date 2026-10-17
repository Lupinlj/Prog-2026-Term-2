using Microsoft.AspNetCore.Mvc;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.DTO;
using RaceDay.Models;
using System.Xml.Serialization;

namespace RaceDay.Tests
{
    public class ApiTests
    {

        private static Event AddEvent(AppDbContext ctx, int organiserId)
        {
            var e = new Event
            {
                EventName = "O1d",
                EventDescription = "d",
                EventDate = DateTime.Now.AddDays(10),
                EventLocation = "Cape Town",
                Distance = 10,
                EventType = "Run",
                OrganiserId = organiserId
            };
            ctx.Events.Add(e); ;ctx.SaveChanges(); return e;
        }
        private static EventDto NewEventDto() => new EventDto
        {
            EventName = "New",
            EventDescription = "d",
            EventDate = DateTime.Now.AddDays(5),
            EventLocation = "Durban",
            Distance = 5,
            EventType = "Run"
        };

        // 1. Registration and login
        [Fact]
        public void Register_ValidOrganiser_SavesAccount()
        {

            var ctx = TestHelper1.CreateContext();
            var c = new AuthController(ctx); TestHelper1.SetUser(c);
            c.Register(new RegisterDto { Name = " O", Email = "o@t.com", Password = "Test123!", Role = "Organiser" });
            Assert.Single(ctx.Organisers);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnOk()
        {
            var ctx = TestHelper1.CreateContext();
            var c = new AuthController(ctx); TestHelper1.SetUser(c);

            c.Register(new RegisterDto
            {
                Name = "O",
                Email = "o@t.com",
                Password = "Test123!",
                Role = "Organiser"
            });
            Assert.IsType<OkObjectResult>(c.Login(new LoginDto
            {
                Email = "o@t.com",
                Password = "Test123!"
            }));
        }

        [Fact]
        public void Login_WrongPassword_ReturnsUnauthorized()
        {
            var ctx = TestHelper1.CreateContext();
            var c = new AuthController(ctx); TestHelper1.SetUser(c);

            c.Register(new RegisterDto
            {
                Name = "O",
                Email = "o@t.com",
                Password = "Test123!",
                Role = "Organiser"
            });
            Assert.IsType<UnauthorizedObjectResult>(c.Login(new LoginDto
            {
                Email = "o@t.com",
                Password = "wrong"
            }));

        }

        // 2. Event management (Organiser)
        [Fact]
        public void CreateEvent_AsOrganiser_SaveEvent()
        {
            var ctx = TestHelper1.CreateContext();
            var c = new EventsController(ctx); TestHelper1.SetUser(c, 1, "Organiser");

            c.CreateEvent(NewEventDto());
            Assert.Single(ctx.Events);
        }

        [Fact]
        public void UpdateEvent_OwnEvent_ChangeName()
        {
            var ctx = TestHelper1.CreateContext();
            var ev = AddEvent(ctx, 1);
            var c = new EventsController(ctx); TestHelper1.SetUser(c, 1, "Organiser");

            c.UpdateEvent(ev.EventId, NewEventDto());
            Assert.Equal("New", ctx.Events.Single().EventName);
        }

        [Fact]
        public void DeleteEvent_OwnEvent_RemovesIt()
        {
            var ctx = TestHelper1.CreateContext();
            var ev = AddEvent(ctx, 1);
            var c = new EventsController(ctx); TestHelper1.SetUser(c, 1, "Organiser");

            c.DeleteEvent(ev.EventId);
            Assert.Empty(ctx.Events);
        }

        // 3. Role rejection and unau
        [Fact]
        public void GetEvents_NotLoggedIn_ReturnsUnauthorized()
        {
            var c = new EventsController(TestHelper1.CreateContext()); TestHelper1.SetUser(c);
            Assert.IsType<UnauthorizedObjectResult>(c.GetEvents());
        }


        [Fact]
        public void CreateEvent_AsParticipant_ReturnsForbidden()
        {
            var ctx = TestHelper1.CreateContext();
            var c = new EventsController(ctx); TestHelper1.SetUser(c, 1, "Participant");
            var result = c.CreateEvent(NewEventDto());
            Assert.Equal(403, Assert.IsType<ObjectResult>(result).StatusCode);
            Assert.Empty(ctx.Events);
        }

        // 4. Enrolments

        [Fact]
        public void Enrol_AsParticipant_IsRecorded()
        {
            var ctx = TestHelper1.CreateContext();
            var ev = AddEvent(ctx, 1);

            var ctg = new Category
            {
                CategoryName = "10km",
                EventId = ev.EventId,
            };
            
            ctx.Categories.Add(ctg); ctx.SaveChanges();
            var c = new EnrolmentsController(ctx); TestHelper1.SetUser(c, 5, "Participant");

            c.Enrol(ev.EventId, new EnrolmentDto
            {
                CategoryId = ctg.CategoryId
            });

            var saved = Assert.Single(ctx.Enrolments);
            Assert.Equal(5, saved.ParticipantId);
            Assert.Equal(ctg.CategoryId, saved.CategoryId);
        }


        [Fact]
        public void Enrol_AsOrganiser_ReturnsForbidden()
        {
            var ctx = TestHelper1.CreateContext();
            var ev = AddEvent(ctx, 1);
            var c = new EnrolmentsController(ctx); TestHelper1.SetUser(c, 1, "Organiser");
            var result = c.Enrol(ev.EventId, new EnrolmentDto
            {
                CategoryId = 1
            });
            Assert.Equal(403, Assert.IsType<ObjectResult>(result).StatusCode);
        }

}
}
