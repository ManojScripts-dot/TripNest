using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;
using Microsoft.AspNetCore.Http; // For session

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
            return View("AgencyLogin"); // Explicitly specify the view name
        }

        // POST: /Agency/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var agency = _context.Users.FirstOrDefault(u =>
                u.Email == email &&
                u.Password == password && // TODO: Hash passwords in production
                u.Role == "Agency");

            if (agency != null)
            {
                HttpContext.Session.SetString("AgencyEmail", agency.Email);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid agency credentials";
            return View("AgencyLogin");
        }

        // GET: /Agency/Dashboard
        public IActionResult Dashboard()
        {
            var agencyEmail = HttpContext.Session.GetString("AgencyEmail");
            if (string.IsNullOrEmpty(agencyEmail))
            {
                return RedirectToAction("Login");
            }

            var agency = _context.Users.FirstOrDefault(u => u.Email == agencyEmail && u.Role == "Agency");
            if (agency == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            ViewData["Title"] = "Dashboard";
            return View(agency);
        }

        // GET: /Agency/ManageTours
        public IActionResult ManageTours()
        {
            var agencyEmail = HttpContext.Session.GetString("AgencyEmail");
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            ViewData["Title"] = "Manage Tours";
            return View();
        }

        // GET: /Agency/Bookings
        public IActionResult Bookings()
        {
            var agencyEmail = HttpContext.Session.GetString("AgencyEmail");
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            ViewData["Title"] = "Bookings";
            return View();
        }

        // GET: /Agency/Feedback
        public IActionResult Feedback()
        {
            var agencyEmail = HttpContext.Session.GetString("AgencyEmail");
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            ViewData["Title"] = "Feedback";
            return View();
        }

        // GET: /Agency/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AgencyEmail");
            return RedirectToAction("Login");
        }
    }
}
