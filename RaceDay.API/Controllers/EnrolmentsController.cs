using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceDay.API.Controllers
{
    [Route("api/enrolments")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/enrolments
        [HttpPost]
        public async Task<IActionResult> Enrol([FromBody] EnrolmentRequestDto dto)
        {
            // 1. Check if user is logged in via Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized("You must be logged in to enrol.");
            }

            // 2. Check if the logged-in user is a Participant
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant")
            {
                return StatusCode(403, "Only Participants can enrol in events.");
            }

            // 3. Prevent double enrolment (409 Conflict)
            var alreadyEnrolled = await _context.Enrolments.AnyAsync(e =>
                e.ParticipantId == userId && e.EventId == dto.EventId);

            if (alreadyEnrolled)
            {
                return Conflict("You are already enrolled in this event.");
            }

            // 4. Create the Enrolment
            var enrolment = new Enrolment
            {
                ParticipantId = userId.Value,
                EventId = dto.EventId,
                EnrolmentDate = DateTime.Now,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Successfully enrolled in the event.", EnrolmentId = enrolment.EnrolmentId });
        }

        // GET: api/enrolments/event/5 (For Organiser Dashboard)
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetEnrolmentsByEvent(int eventId)
        {
            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Where(e => e.EventId == eventId)
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/enrolments/my-enrolments (For Participant Dashboard)
        [HttpGet("my-enrolments")]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetMyEnrolments()
        {
            // 1. Get the logged-in user's ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized("You must be logged in.");
            }

            // 2. Fetch enrolments for this user, and include the Event details
            var enrolments = await _context.Enrolments
                .Include(e => e.Event) // This links the enrolment to the Event table
                .Where(e => e.ParticipantId == userId)
                .ToListAsync();

            return Ok(enrolments);
        }
    }

    // Simple DTO to capture the request body
    public class EnrolmentRequestDto
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }
}