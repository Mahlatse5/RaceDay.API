using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.API.Tests
{
    public class EventsControllerTests
    {
        private RaceDayDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        [Fact]
        public async Task PostEvent_ByParticipant_Returns403Forbidden()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new EventsController(context);

            // Setup fake session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            httpContext.Session.SetInt32("UserId", 1);
            httpContext.Session.SetString("Role", "Participant");
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var newEvent = new Event
            {
                Name = "Fake Event",
                Location = "Fake Location",
                Distance = 5,
                EventDate = System.DateTime.Now,
                EventTypeId = 1
            };

            // Act
            var result = await controller.PostEvent(newEvent);

            // Assert (Safe check)
            
            var objectResult = result.Result as ObjectResult;

            Assert.NotNull(objectResult); // Proves it returned an ObjectResult
            Assert.Equal(403, objectResult.StatusCode); // Proves it was Forbidden
        }
    }
}