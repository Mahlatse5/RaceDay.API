using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceDay.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/categories/event/5 (Get categories for a specific event)
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesByEvent(int eventId)
        {
            var categories = await _context.Categories
                .Where(c => c.EventId == eventId)
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/categories (Organiser adds a new category)
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            // Check if user is an Organiser
            var role = HttpContext.Session.GetString("Role");
            if (role == null || !role.Equals("Organiser", System.StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(403, "Only Organisers can create categories.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoriesByEvent), new { eventId = category.EventId }, category);
        }
    }
}