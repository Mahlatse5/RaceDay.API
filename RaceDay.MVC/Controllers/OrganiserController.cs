using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Models; // This links to your Part 2 API models
using System.Net.Http.Json;

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
            // Check if user is logged in and is an Organiser
            if (HttpContext.Session.GetInt32("UserId") == null ||
                HttpContext.Session.GetString("Role") != "Organiser")
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            // Fetch all events from the API
            var events = await client.GetFromJsonAsync<List<Event>>("api/events");

            return View(events);
        }
    }
}