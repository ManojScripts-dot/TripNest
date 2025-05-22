using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;

namespace TripNest.Controllers
{
    public class AgencyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgencyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Agency/Login
        public IActionResult Login()
        {
            return View("AgencyLogin");
        }

        // POST: /Agency/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var agency = _context.Users.FirstOrDefault(u =>
                u.Email == email &&
                u.Password == password && // TODO: Use hashed passwords in production
                u.Role == "Agency");

            if (agency != null)
            {
                // ✅ Set cookie
                Response.Cookies.Append("AgencyEmail", agency.Email, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true
                });

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid agency credentials";
            return View("AgencyLogin");
        }

        // GET: /Agency/Dashboard
        public IActionResult Dashboard()
        {
            var agencyEmail = Request.Cookies["AgencyEmail"];
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            var agency = _context.Users.FirstOrDefault(u => u.Email == agencyEmail && u.Role == "Agency");
            if (agency == null)
            {
                Response.Cookies.Delete("AgencyEmail");
                return RedirectToAction("Login");
            }

            ViewData["Title"] = "Dashboard";
            return View(agency);
        }

        // GET: /Agency/ManageTours
        public IActionResult ManageTours()
        {
            var agencyEmail = Request.Cookies["AgencyEmail"];
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            var tours = _context.Tours.ToList(); // You can filter by agency later
            ViewData["Title"] = "Manage Tours";
            return View(tours);
        }

        // GET: /Agency/Bookings
        public IActionResult Bookings()
        {
            var agencyEmail = Request.Cookies["AgencyEmail"];
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            ViewData["Title"] = "Bookings";
            return View();
        }

        // GET: /Agency/Feedback
        public IActionResult Feedback()
        {
            var agencyEmail = Request.Cookies["AgencyEmail"];
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            ViewData["Title"] = "Feedback";
            return View();
        }

        // GET: /Agency/Logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AgencyEmail");
            return RedirectToAction("Login");
        }
    }
}
