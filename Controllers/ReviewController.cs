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
                return BadRequest("Invalid booking.");

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            bool exists = _context.Reviews.Any(r => r.BookingId == bookingId && r.UserId == userId);
            if (exists)
                return BadRequest("You’ve already submitted a review.");

            var review = new Review { BookingId = bookingId, UserId = userId };
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Review model)
        {
            if (!ModelState.IsValid)
            {

                 foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
    {
        Console.WriteLine("Validation error: " + error.ErrorMessage);
    }
                // Optionally log or inspect validation errors here:
                // var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(model);
            }

            var booking = _context.Bookings.FirstOrDefault(b => b.Id == model.BookingId);
            if (booking == null || !booking.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid booking.");

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            bool exists = _context.Reviews.Any(r => r.BookingId == model.BookingId && r.UserId == userId);
            if (exists)
                return BadRequest("Review already submitted.");

            model.UserId = userId;
            model.CreatedAt = DateTime.Now;

            Console.WriteLine($"Posted Review - BookingId: {model.BookingId}, Stars: {model.Stars}, Message: {model.Message}");

            _context.Reviews.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Profile", "Account");
        }
    }
}