using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;
using Microsoft.AspNetCore.Http;

namespace TripNest.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TourController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAgencyLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("AgencyEmail"));
        }

        public IActionResult Index()
        {
            if (!IsAgencyLoggedIn())
                return RedirectToAction("Login", "Agency");

            var tours = _context.Tours.ToList();
            return View(tours);
        }

        public IActionResult Create()
        {
            if (!IsAgencyLoggedIn())
                return RedirectToAction("Login", "Agency");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Tour tour)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToAction("Login", "Agency");

            if (ModelState.IsValid)
            {
                _context.Tours.Add(tour);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(tour);
        }

        // TODO: Add Edit and Delete with similar session checks
    }
}
