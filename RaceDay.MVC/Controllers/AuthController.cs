using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.ViewModels;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    //  This is an MVC controller.
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();

                // Store user details in MVC Session
                HttpContext.Session.SetInt32("UserId", responseData.GetProperty("userId").GetInt32());
                HttpContext.Session.SetString("Role", responseData.GetProperty("role").GetString()!);

                // Redirect based on role
                if (responseData.GetProperty("role").GetString() == "Organiser")
                    return RedirectToAction("Index", "Organiser");
                else
                    return RedirectToAction("Index", "Home"); // Participant goes to home
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. Email may already be in use.");
            return View(model);
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Wipe the session
            return RedirectToAction("Index", "Home");
        }
    }
}