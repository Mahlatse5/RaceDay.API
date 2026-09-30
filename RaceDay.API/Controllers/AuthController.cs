using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public AuthController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return Conflict("Email is already in use.");
            }

            // 2. Hash the password (Never store plain text!)
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 3. Create the new user
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = dto.Role,
                PhoneNumber = dto.PhoneNumber
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // 1. Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // 2. Verify the password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            // 3. Set Session Variables 
            HttpContext.Session.SetInt32("UserId", user.UserID);
            HttpContext.Session.SetString("Role", user.Role);

            return Ok(new { Message = "Login successful", UserId = user.UserID, Role = user.Role });
        }
    }
}