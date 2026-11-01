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
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events (Public - Anyone can view)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            // Include EventType so the JSON response shows the type name, not just the ID
            return await _context.Events.Include(e => e.EventType).ToListAsync();
        }

        // GET: api/events/5 (Public - Anyone can view details)
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var @event = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }

        // POST: api/events (Restricted - Organisers ONLY)
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(EventCreateDto dto)
        {
            // 1. Check if user is logged in via Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized("You must be logged in to create an event.");
            }

            // 2. Check if the logged-in user is an Organiser
            var role = HttpContext.Session.GetString("Role");
            if (role == null || !role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(403, "Only Organisers can create events."); // 403 Forbidden
            }

            // Manually create the Event object using the simple DTO data.
            // This completely bypasses the strict validation on the main Event model's navigation properties!
            var newEvent = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventTypeId = dto.EventTypeId,
                OrganiserId = userId.Value // Automatically assign the logged-in user
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
        }
    }
}