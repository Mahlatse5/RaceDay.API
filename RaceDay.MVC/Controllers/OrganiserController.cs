using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models;
using RaceDay.MVC.ViewModels;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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
                    var jsonString = await response.Content.ReadAsStringAsync();
                    events = JsonSerializer.Deserialize<List<Event>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Event>();
                }
                else
                {
                    ViewBag.ErrorMessage = "API returned error: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Connection Error: " + ex.Message;
            }

            return View(events);
        }

        // GET: Organiser/Enrolments/5 
        public async Task<IActionResult> Enrolments(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var response = await client.GetAsync($"api/enrolments/event/{id}");

            List<Enrolment> enrolments = new List<Enrolment>();
            if (response.IsSuccessStatusCode)
            {
                enrolments = await response.Content.ReadFromJsonAsync<List<Enrolment>>() ?? new List<Enrolment>();
            }

            ViewBag.EventId = id;
            return View(enrolments);
        }

        // GET: Organiser/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        // POST: Organiser/Create (With Local Image Upload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string imageUrl = "";

            // 1. Handle the Image Upload Locally
            if (model.BannerImage != null && model.BannerImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BannerImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.BannerImage.CopyToAsync(stream);
                }

                imageUrl = $"/uploads/{fileName}";
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // 2. Send the data to the API
            var eventData = new
            {
                Name = model.Name,
                Description = model.Description,
                EventDate = model.EventDate,
                Location = model.Location,
                Distance = model.Distance,
                EventTypeId = model.EventTypeId,
                BannerImageUrl = imageUrl
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

        // GET: Organiser/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var response = await client.GetAsync($"api/events/{id}");

            if (response.IsSuccessStatusCode)
            {
                var @event = await response.Content.ReadFromJsonAsync<Event>();
                // Map to ViewModel if needed, or return View directly
                return View(@event);
            }

            return NotFound();
        }

        // POST: Organiser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event model)
        {
            if (id != model.EventId) return NotFound();

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"api/events/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Organiser/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var response = await client.DeleteAsync($"api/events/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}