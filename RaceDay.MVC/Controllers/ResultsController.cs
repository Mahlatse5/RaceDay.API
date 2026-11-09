using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.MVC.Models;
using RaceDay.API.Models; // Keep your namespace here

namespace RaceDay.MVC.Controllers
{
    public class ResultsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ResultsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // GET: Results/Enter/5
        [HttpGet]
        public async Task<IActionResult> Enter(int eventId)
        {
            var client = _clientFactory.CreateClient("RaceDayAPI");
            // We pass the EventId to the view so it knows which event we are grading
            ViewBag.EventId = eventId;
            return View();
        }

        // POST: Results/Enter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enter(int enrolmentId, string finishTime, int position)
        {
            var client = _clientFactory.CreateClient("RaceDayAPI");

            // 1. Update Finish Time
            await client.PutAsJsonAsync($"api/enrolments/{enrolmentId}/finish", finishTime);

            // Note: You can add a similar API call for Position later if needed.

            return RedirectToAction("Index", "Home"); // Goes back to dashboard
        }

        
        // GET: Results/MyResults
        [HttpGet]
        public async Task<IActionResult> MyResults()
        {
            var client = _clientFactory.CreateClient("RaceDayAPI");

            // We get the Participant ID from the Session
            var participantId = HttpContext.Session.GetInt32("ParticipantId");

            if (participantId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetAsync($"api/enrolments/participant/{participantId}");

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadFromJsonAsync<List<Enrolment>>();
                return View(results);
            }

            return View("Error");
        }
    }
    }