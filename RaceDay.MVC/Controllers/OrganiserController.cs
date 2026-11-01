using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using RaceDay.MVC.ViewModels;
using RaceDay.API.Models; // Ensure this matches your models namespace

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrganiserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Organiser/Index (The Dashboard)
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            List<Event> events = new List<Event>();

            try
            {
                var response = await client.GetAsync("api/events");
                if (response.IsSuccessStatusCode)
                {
                    events = await response.Content.ReadFromJsonAsync<List<Event>>() ?? new List<Event>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
            }

            return View(events);
        }

        // GET: Organiser/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        // POST: Organiser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // Send ONLY the IDs and text. No nested objects!
            var eventData = new
            {
                Name = model.Name,
                Description = model.Description,
                EventDate = model.EventDate,
                Location = model.Location,
                Distance = model.Distance,
                EventTypeId = model.EventTypeId,
                OrganiserId = HttpContext.Session.GetInt32("UserId")
            };

            var json = JsonSerializer.Serialize(eventData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/events", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "API Error: " + errorContent);
            return View(model);
        }
    }
}