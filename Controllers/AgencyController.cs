using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;
using Microsoft.AspNetCore.Http; // For CookieOptions
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TripNest.Models.ViewModels;



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

        // GET: /Agency/Register
        public IActionResult Register()
        {
            return View("AgencyRegister");
        }

        // POST: /Agency/Register
        [HttpPost]
        public IActionResult Register(AgencyRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AgencyRegister", model);
            }

            // Check if agency with this email already exists
            var existingAgency = _context.Agencies.FirstOrDefault(a => a.Email == model.Email);
            if (existingAgency != null)
            {
                ModelState.AddModelError("Email", "An agency with this email already exists.");
                return View("AgencyRegister", model);
            }

            // Create new agency
            var agency = new Agency
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password // In production, this should be hashed
            };

            _context.Agencies.Add(agency);
            _context.SaveChanges();

            // Set cookies for immediate login
            Response.Cookies.Append("AgencyEmail", agency.Email, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });

            Response.Cookies.Append("AgencyId", agency.Id.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            });

            return RedirectToAction("Dashboard");
        }

        // POST: /Agency/Login
        [HttpPost]
        public IActionResult Login(AgencyLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AgencyLogin", model);
            }

            var agency = _context.Agencies.FirstOrDefault(a =>
     a.Email == model.Email &&
     a.Password == model.Password);

            if (agency != null)
            {
                Response.Cookies.Append("AgencyEmail", agency.Email, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true
                });

                Response.Cookies.Append("AgencyId", agency.Id.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true
                });

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid agency credentials";
            return View("AgencyLogin", model);
        }

        // GET: /Agency/Dashboard
        public IActionResult Dashboard()
        {
            var agencyEmail = Request.Cookies["AgencyEmail"];
            var agencyId = Request.Cookies["AgencyId"];

            Console.WriteLine($"AgencyEmail cookie: {agencyEmail}");
            Console.WriteLine($"AgencyId cookie: {agencyId}");


            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            var agency = _context.Agencies.FirstOrDefault(a => a.Email == agencyEmail);

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
            var agencyIdCookie = Request.Cookies["AgencyId"];

            if (string.IsNullOrEmpty(agencyEmail) || string.IsNullOrEmpty(agencyIdCookie) || !int.TryParse(agencyIdCookie, out int agencyId))
            {
                return RedirectToAction("Login");
            }

            // Debug: Log the agency ID
            Console.WriteLine($"DEBUG: Looking for bookings for AgencyId: {agencyId}");

            // Get all tours for this agency first
            var agencyTours = _context.Tours.Where(t => t.AgencyId == agencyId).ToList();
            Console.WriteLine($"DEBUG: Found {agencyTours.Count} tours for agency {agencyId}");
            
            // If no tours found for this agency, assign some existing tours to this agency
            if (agencyTours.Count == 0)
            {
                Console.WriteLine($"DEBUG: No tours found for agency {agencyId}, assigning existing tours...");
                var existingTours = _context.Tours.Where(t => t.AgencyId == null || t.AgencyId == 1).ToList();
                foreach (var tour in existingTours)
                {
                    tour.AgencyId = agencyId;
                    Console.WriteLine($"DEBUG: Assigned tour {tour.Id} - {tour.Title} to agency {agencyId}");
                }
                _context.SaveChanges();
                agencyTours = _context.Tours.Where(t => t.AgencyId == agencyId).ToList();
                Console.WriteLine($"DEBUG: After assignment, found {agencyTours.Count} tours for agency {agencyId}");
            }
            
            foreach (var tour in agencyTours)
            {
                Console.WriteLine($"DEBUG: Tour {tour.Id} - {tour.Title} (AgencyId: {tour.AgencyId})");
            }

            // Get bookings for tours where Tour.AgencyId == agencyId, guarding against null Tour
            var bookings = _context.Bookings
                .Include(b => b.Tour)  // Include tour details
                .Include(b => b.User)  // Include user details if needed
                .Where(b => b.Tour != null && b.Tour.AgencyId == agencyId)
                .ToList();

            Console.WriteLine($"DEBUG: Found {bookings.Count} bookings for agency {agencyId}");

            ViewData["Title"] = "Bookings";
            return View(bookings);
        }


        // GET: /Agency/ViewBooking/5
        public IActionResult ViewBooking(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.User)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: /Agency/ViewBooking/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewBooking(int id, string status)
        {


            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return NotFound();

            booking.Status = status;
            _context.SaveChanges();

            return RedirectToAction("Bookings");
        }


        public IActionResult Review()
        {
            var agencyEmail = Request.Cookies["AgencyEmail"];
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            var agency = _context.Agencies.FirstOrDefault(a => a.Email == agencyEmail);
            if (agency == null)
            {
                Response.Cookies.Delete("AgencyEmail");
                return RedirectToAction("Login");
            }

            // 🔽 This is the correct place for your code
            var reviews = _context.Reviews
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Tour)
                .Where(r => r.Booking.Tour != null && r.Booking.Tour.AgencyId == agency.Id)
                .Select(r => new ReviewViewModel
                {
                    TourTitle = r.Booking.Tour!.Title,
                    TourDestination = r.Booking.Tour.Destination,
                    Content = r.Message ?? "",
                    Rating = r.Stars,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            foreach (var r in reviews)
            {
                Console.WriteLine($"DEBUG: Title={r.TourTitle}, Destination={r.TourDestination}");
            }


            ViewData["Title"] = "Reviews";
            return View(reviews); // ← Pass the list to your Razor View
        }


        public IActionResult ContactMessages()
        {
            var agencyEmail = Request.Cookies["AgencyEmail"];
            if (string.IsNullOrEmpty(agencyEmail))
                return RedirectToAction("Login");

            var messages = _context.ContactMessages
                .OrderByDescending(m => m.SubmittedAt)
                .ToList();

            ViewData["Title"] = "Contact Messages";
            return View(messages);
        }



        // GET: /Agency/Logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AgencyEmail");
            return RedirectToAction("Login");
        }
    }
}
