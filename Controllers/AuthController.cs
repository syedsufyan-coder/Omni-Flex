using Microsoft.AspNetCore.Mvc;
using OmniFlex.Models.ViewModels;
using OmniFlex.Models.Repositories.Admin;
using BCrypt.Net;

namespace OmniFlex.Controllers
{
    public class AuthController(IUserRepository users) : Controller
    {
        private readonly IUserRepository _users = users;
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Login(string role = "student")
        {
            var model = new LoginViewModel { Role = role };
            return View(model);
        }

        [HttpPost]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _users.GetActiveLoginUserAsync(model.UserId);

            /*Critical Section: MUST BE CHANGED IN PRODUCTION
            For Development: Passwords are stored in plain text for easy testing. In production, use hashed passwords and verify using BCrypt.
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid ID or password");
                return View(model);
            }*/

            if(user == null || user.Role == null || user.Role.ToLowerInvariant() != model.Role.ToLowerInvariant())
            {
                ModelState.AddModelError("", "Invalid ID or password.");
                return View(model);
            }

            if (user == null || user.PasswordHash != model.Password)
            {
                ModelState.AddModelError("", "Invalid ID or password.");
                return View(model);
            }
            // Store in session
            HttpContext.Session.SetString("UserId",   user.UserId);
            HttpContext.Session.SetString("UserName", user.FirstName + " " + user.LastName);
            HttpContext.Session.SetString("Role",     user.Role);

            return (user.Role ?? string.Empty).ToLowerInvariant() switch
            {
                "admin"      => RedirectToAction("Dashboard", "Admin"),
                "instructor" => RedirectToAction("Dashboard", "Instructor"),
                "ta"         => RedirectToAction("Dashboard", "Instructor"),
                "student"    => RedirectToAction("Dashboard", "Student"),
                _             => RedirectToAction("Index", "Home")
            };
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
