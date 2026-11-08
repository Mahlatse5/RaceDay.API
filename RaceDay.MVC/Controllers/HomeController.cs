using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.MVC.Models;
using RaceDay.API.Models; // Added to access the Event model from the API
using System.Diagnostics;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory; // Added HttpClient

        // Updated constructor to inject HttpClient
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        // Updated Index method to fetch events from the API
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            List<Event> events = new List<Event>();

            try
            {
                // Fetch events from the API
                var response = await client.GetAsync("api/events");

                if (response.IsSuccessStatusCode)
                {
                    events = await response.Content.ReadFromJsonAsync<List<Event>>() ?? new List<Event>();
                }
            }
            catch (Exception ex)
            {
                // If the API is down, we just show an empty list instead of crashing
                ViewBag.ErrorMessage = "Could not connect to API: " + ex.Message;
            }

            // Pass the list of events to the View
            return View(events);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}