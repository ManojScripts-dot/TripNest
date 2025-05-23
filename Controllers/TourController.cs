using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;

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
    }
}
