using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;
using System;
using System.Linq;

namespace TripNest.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null || !booking.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
                return View("Error", new ErrorViewModel { ErrorMessage = "Invalid booking. The booking does not exist or is not completed." });

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            bool exists = _context.Reviews.Any(r => r.BookingId == bookingId && r.UserId == userId);
            if (exists)
                return View("Error", new ErrorViewModel { ErrorMessage = "You've already submitted a review for this booking." });

            var review = new Review { BookingId = bookingId, UserId = userId };
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Review model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var booking = _context.Bookings.FirstOrDefault(b => b.Id == model.BookingId);
            if (booking == null || !booking.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
                return View("Error", new ErrorViewModel { ErrorMessage = "Invalid booking. The booking does not exist or is not completed." });

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            bool exists = _context.Reviews.Any(r => r.BookingId == model.BookingId && r.UserId == userId);
            if (exists)
                return View("Error", new ErrorViewModel { ErrorMessage = "Review already submitted for this booking." });

            model.UserId = userId;
            model.CreatedAt = DateTime.Now;

            _context.Reviews.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Profile", "Account");
        }
    }

    public class ErrorViewModel
    {
        public string ErrorMessage { get; set; }
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}