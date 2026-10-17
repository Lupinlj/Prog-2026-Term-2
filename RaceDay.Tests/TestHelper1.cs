using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;

namespace RaceDay.Tests
{
    public static class TestHelper1
    {
        public static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new AppDbContext(options);
        }

        public static void SetUser(ControllerBase controller, int? userId = null, string? role = null)
        {
            var session = new TestSessions();
            if (userId != null) session.SetInt32("UserId", userId.Value);
            if (role != null) session.SetString("Role", role);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Session = session}
            };
        }
    }
}
