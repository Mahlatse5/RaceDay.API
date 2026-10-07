using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.API.Tests
{
    public class EnrolmentsControllerTests
    {
        private RaceDayDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        // TEST: Prove a logged-in Participant can successfully enrol
        [Fact]
        public async Task Enrol_ValidParticipant_ReturnsOk()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new EnrolmentsController(context);

            // Setup fake session for a Participant
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            httpContext.Session.SetInt32("UserId", 1);
            httpContext.Session.SetString("Role", "Participant");
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Create the DTO for the request
            var dto = new EnrolmentRequestDto
            {
                EventId = 101,
                CategoryId = 202
            };

            // Act
            var result = await controller.Enrol(dto);

            // Assert 
            
            var objectResult = result as OkObjectResult;

            Assert.NotNull(objectResult);
            Assert.Equal(200, objectResult.StatusCode);
        }
    }
}