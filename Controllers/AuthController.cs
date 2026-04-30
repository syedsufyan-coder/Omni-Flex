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
        public IActionResult Login(string role = "student")
        {
            var model = new LoginViewModel { Role = role };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _users.GetByIdAsync(model.UserId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid ID or password");
                return View(model);
            }

            // Store in session
            HttpContext.Session.SetString("UserId",   user.UserId);
            HttpContext.Session.SetString("UserName", user.FirstName + " " + user.LastName);
            HttpContext.Session.SetString("Role",     user.Role);

            return user.Role switch
            {
                "Admin"      => RedirectToAction("Dashboard", "Admin"),
                "Instructor" => RedirectToAction("Dashboard", "Admin"),
                "TA"         => RedirectToAction("Dashboard", "Admin"),
                "Student"    => RedirectToAction("Dashboard", "Admin"),
                _            => RedirectToAction("Index", "Home")
            };
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
