using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;  // For accessing claims
using System.Threading.Tasks;
using System;

namespace TripNest.Controllers
{
    [Authorize]  // Protect all actions (adjust if needed)
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show booking form for a specific tour
        public IActionResult Create(int tourId)
        {
            var tour = _context.Tours.Find(tourId);
            if (tour == null)
                return NotFound();

            var booking = new Booking
            {
                TourId = tour.Id,
                PreferredTourDate = DateTime.Today
            };

            ViewBag.Tour = tour;
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            // Debug: List all claims to the console or debug output (optional)
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
            }

            var tour = await _context.Tours.FindAsync(booking.TourId);
            if (tour == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Get logged-in user's email from claims
                var emailClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized();

                // Look up user in the database by email to get the UserId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
                if (user == null)
                    return Unauthorized();

                // Assign UserId to booking
                booking.UserId = user.Id;
                booking.TotalAmount = tour.Price * booking.NumberOfGuests;
                booking.BookingDate = DateTime.Now;

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmation", new { id = booking.Id });
            }

            ViewBag.Tour = tour;
            return View(booking);
        }

        public IActionResult Confirmation(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.User)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }
    }
}
