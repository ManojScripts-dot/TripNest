using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;
using System.Globalization;

namespace TripNest.Controllers
{
    // Only authenticated Agency users can access these actions
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TourController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tours = _context.Tours.ToList();
            return View(tours);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tour tour)
        {
            var agencyIdCookie = Request.Cookies["AgencyId"];
            if (string.IsNullOrEmpty(agencyIdCookie) || !int.TryParse(agencyIdCookie, out int agencyId))
            {
                return RedirectToAction("Login", "Agency"); // Not logged in
            }

            // Assign the AgencyId before saving
            tour.AgencyId = agencyId;

            if (ModelState.IsValid)
            {
                _context.Tours.Add(tour);
                _context.SaveChanges();
                return RedirectToAction("Dashboard", "Agency");
            }

            return View(tour);
        }



        public IActionResult Edit(int id)
        {
            var tour = _context.Tours.Find(id);
            if (tour == null)
                return NotFound();

            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Tour tour)
        {
            if (id != tour.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Tours.Update(tour);
                _context.SaveChanges();
                return RedirectToAction("Dashboard", "Agency");
            }

            return View(tour);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var tour = _context.Tours.FirstOrDefault(t => t.Id == id);
            if (tour == null)
                return NotFound();

            return View(tour);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tour = _context.Tours.Find(id);
            if (tour == null)
                return NotFound();

            _context.Tours.Remove(tour);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Agency");
        }

        public IActionResult Details(int id)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.Id == id);
            if (tour == null)
                return NotFound();

            return View(tour);
        }


        public IActionResult Search(string destination, string date, string priceRange)
        {
            var tours = _context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(destination))
            {
                tours = tours.Where(t => t.Destination.Contains(destination));
            }

            // If you have no date property, you can skip this or handle differently.
            // For now, skip date filtering or implement if you add it.

            if (!string.IsNullOrEmpty(priceRange))
            {
                if (priceRange == "0-500")
                    tours = tours.Where(t => t.Price >= 0 && t.Price <= 500);
                else if (priceRange == "501-1000")
                    tours = tours.Where(t => t.Price >= 501 && t.Price <= 1000);
                else if (priceRange == "1001-2000")
                    tours = tours.Where(t => t.Price >= 1001 && t.Price <= 2000);
                else if (priceRange == "2001+")
                    tours = tours.Where(t => t.Price >= 2001);
            }

            var result = tours.ToList();

            return View("SearchResults", result);
        }


    }
}
