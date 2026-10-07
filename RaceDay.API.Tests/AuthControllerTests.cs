using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.API.Tests
{
    public class AuthControllerTests
    {
        // Helper method to create a fresh, fake database for every test
        private RaceDayDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        // TEST 1: Prove that a valid user can register successfully
        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange (Set up the fake database and controller)
            var context = CreateInMemoryContext();
            var controller = new AuthController(context);
            var dto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                Password = "Password123!",
                Role = "Participant"
            };

            // Act (Run the register method)
            var result = await controller.Register(dto);

            // Assert (Check if it returned a 200 OK)
            Assert.IsType<OkObjectResult>(result);
        }

        // TEST 2: Prove that duplicate emails are rejected
        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var context = CreateInMemoryContext();

            // Pre-populate the fake database with an existing user
            context.Users.Add(new User
            {
                FirstName = "Existing",
                LastName = "User",
                Email = "test@test.com",
                PasswordHash = "fakehash",
                Role = "Participant"
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(context);
            var dto = new RegisterDto
            {
                FirstName = "New",
                LastName = "User",
                Email = "test@test.com", // Same email!
                Password = "Password123!",
                Role = "Participant"
            };

            // Act
            var result = await controller.Register(dto);

            // Assert (Check if it returned a 409 Conflict)
            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}