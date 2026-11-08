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
        // GET: Organiser/Index (The Dashboard)
        public async Task<IActionResult> Index()
        {
            // Security check
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            List<Event> events = new List<Event>();

            try
            {
                // 1. Fetch from API
                var response = await client.GetAsync("api/events");

                // 2. Check if successful
                if (response.IsSuccessStatusCode)
                {
                    // 3. Read the JSON and convert to List<Event>
                    var jsonString = await response.Content.ReadAsStringAsync();
                    events = JsonSerializer.Deserialize<List<Event>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Event>();
                }
                else
                {
                    // If API fails, show the error on the dashboard
                    ViewBag.ErrorMessage = "API returned error: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Connection Error: " + ex.Message;
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
        // POST: Organiser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string imageUrl = "";

            // 1. Handle the Image Upload Locally
            if (model.BannerImage != null && model.BannerImage.Length > 0)
            {
                // Creates a unique filename so we don't overwrite existing images
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BannerImage.FileName);

                // Define the path: wwwroot/uploads/filename.jpg
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.BannerImage.CopyToAsync(stream);
                }

                // Create the URL that the browser can use to see the image
                imageUrl = $"/uploads/{fileName}";
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // 2. Send the data (including the image URL) to the API as JSON
            var eventData = new
            {
                Name = model.Name,
                Description = model.Description,
                EventDate = model.EventDate,
                Location = model.Location,
                Distance = model.Distance,
                EventTypeId = model.EventTypeId,
                BannerImageUrl = imageUrl // Send the local path!
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
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var response = await client.GetAsync($"api/events/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var eventObj = await response.Content.ReadFromJsonAsync<Event>();

            // Map to ViewModel
            var model = new EventCreateViewModel // Reusing the create model for simplicity
            {
                Name = eventObj.Name,
                Description = eventObj.Description,
                EventDate = eventObj.EventDate,
                Location = eventObj.Location,
                Distance = eventObj.Distance,
                EventTypeId = eventObj.EventTypeId
            };
            ViewBag.EventId = id; // Pass ID to view
            return View(model);
        }

        // POST: Organiser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var eventData = new
            {
                Name = model.Name,
                Description = model.Description,
                EventDate = model.EventDate,
                Location = model.Location,
                Distance = model.Distance,
                EventTypeId = model.EventTypeId
            };

            var json = JsonSerializer.Serialize(eventData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/events/{id}", content);

            if (response.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Failed to update event.");
            ViewBag.EventId = id;
            return View(model);
        }

        // GET: Organiser/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var response = await client.GetAsync($"api/events/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var eventObj = await response.Content.ReadFromJsonAsync<Event>();
            return View(eventObj);
        }

        // POST: Organiser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var response = await client.DeleteAsync($"api/events/{id}");

            if (response.IsSuccessStatusCode) return RedirectToAction("Index");

            return RedirectToAction("Index"); // Fallback
        }
    }
}