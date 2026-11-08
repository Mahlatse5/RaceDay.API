using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync()
using RaceDay.API.Data;
using RaceDay.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceDay.API.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            try
            {
                // Fetch events without includes to avoid crashes
                var events = await _context.Events.ToListAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "API Error: " + ex.Message);
            }
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(EventCreateDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized("You must be logged in.");

            var role = HttpContext.Session.GetString("Role");
            if (role == null || !role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, "Only Organisers can create events.");

            var newEvent = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventTypeId = dto.EventTypeId,
                OrganiserId = userId.Value,
                BannerImageUrl = dto.BannerImageUrl
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
        }

        // PUT: api/events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, EventUpdateDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var role = HttpContext.Session.GetString("Role");
            if (role == null || !role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, "Only Organisers can edit events.");

            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null) return NotFound();

            existingEvent.Name = dto.Name;
            existingEvent.Description = dto.Description;
            existingEvent.EventDate = dto.EventDate;
            existingEvent.Location = dto.Location;
            existingEvent.Distance = dto.Distance;
            existingEvent.EventTypeId = dto.EventTypeId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var role = HttpContext.Session.GetString("Role");
            if (role == null || !role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, "Only Organisers can delete events.");

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}