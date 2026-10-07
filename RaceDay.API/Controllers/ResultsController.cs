using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/results")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/results/{enrolmentId} (Public - Anyone can view a result)
        [HttpGet("{enrolmentId}")]
        public async Task<ActionResult<Result>> GetResult(int enrolmentId)
        {
            var result = await _context.Results
                .Include(r => r.Enrolment)
                .ThenInclude(e => e.Participant)
                .Include(r => r.Enrolment)
                .ThenInclude(e => e.Event)
                .FirstOrDefaultAsync(r => r.EnrolmentId == enrolmentId);

            if (result == null)
            {
                return NotFound("Result not found for this enrolment.");
            }

            return result;
        }

        // POST: api/results (Restricted - Organisers ONLY)
        [HttpPost]
        public async Task<IActionResult> PostResult([FromBody] ResultDto dto)
        {
            // 1. Check if user is logged in via Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized("You must be logged in to capture results.");
            }

            // 2. Check if the logged-in user is an Organiser
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
            {
                return StatusCode(403, "Only Organisers can capture race results.");
            }

            // 3. Verify the enrolment actually exists
            var enrolment = await _context.Enrolments.FindAsync(dto.EnrolmentId);
            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            // 4. Prevent duplicate results for the same enrolment
            var existingResult = await _context.Results.AnyAsync(r => r.EnrolmentId == dto.EnrolmentId);
            if (existingResult)
            {
                return Conflict("A result has already been recorded for this enrolment.");
            }

            // 5. Create the Result
            var result = new Result
            {
                EnrolmentId = dto.EnrolmentId,
                FinishTime = dto.FinishTime, 
                Position = dto.Position
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Result recorded successfully.", ResultId = result.ResultId });
        }
    }

    // DTO for capturing results
    public class ResultDto
    {
        public int EnrolmentId { get; set; }

        // Expects format "hh:mm:ss" (e.g., "01:30:45")
        public TimeSpan FinishTime { get; set; }

        public int Position { get; set; }
    }
}