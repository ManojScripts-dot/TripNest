using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;

namespace TripNest.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            // Return the UserRegister.cshtml view explicitly
            return View("UserRegister");
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // In real apps, hash the password!
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }

            // Return UserRegister.cshtml with validation errors and user model
            return View("UserRegister", user);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // Return the UserLogin.cshtml view explicitly
            return View("UserLogin");
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                // In real apps, use proper authentication/claims
                HttpContext.Session.SetString("UserEmail", user.Email);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            // Return UserLogin.cshtml view with error message
            return View("UserLogin");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
