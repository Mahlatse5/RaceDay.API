using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models;
using RaceDay.MVC.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EnrolmentsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Show confirmation screen before enrolling
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            // 1. Security Check
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // 2. Fetch event details to show on the confirmation screen
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var eventDetails = await client.GetFromJsonAsync<Event>($"api/events/{id}");

            if (eventDetails == null) return NotFound();

            return View(eventDetails);
        }

        // POST: Actually send the enrolment to the API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // 3. Create the payload. 

            var enrolmentData = new
            {
                EventId = id,
                UserId = userId,
                EnrolmentDate = DateTime.Now
            };

            var json = JsonSerializer.Serialize(enrolmentData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/enrolments", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Successfully enrolled in the event!";
                return RedirectToAction("Index", "Home");
            }

            // If it fails, show the error
            var error = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = "Enrolment failed: " + error;
            return RedirectToAction("Index", "Home");
        }

        // GET: Enrolments/MyEnrolments
        [HttpGet]
        public async Task<IActionResult> MyEnrolments()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // Get the Participant ID from the Session
            var participantId = HttpContext.Session.GetInt32("ParticipantId");

            if (participantId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetAsync($"api/enrolments/my-enrolments/{participantId}");

            if (response.IsSuccessStatusCode)
            {
                var enrolments = await response.Content.ReadFromJsonAsync<List<Enrolment>>();
                return View(enrolments);
            }

            return View("Error");
        }
    }
}