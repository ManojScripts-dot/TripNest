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
                return RedirectToAction("Dashboard", "Agency");

            }

            return View(tour);
        }

        // TODO: Add Edit and Delete with similar session checks
        // GET: Tour/Edit/5
        public IActionResult Edit(int id)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToAction("Login", "Agency");

            var tour = _context.Tours.Find(id);
            if (tour == null)
                return NotFound();

            return View(tour);
        }

        // POST: Tour/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Tour tour)


        {
            if (!IsAgencyLoggedIn())
                return RedirectToAction("Login", "Agency");

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
        // GET: Tour/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var tour = _context.Tours.FirstOrDefault(t => t.Id == id);
            if (tour == null)
                return NotFound();

            return View(tour);  // show confirmation view with tour info
        }

        // POST: Tour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tour = _context.Tours.Find(id);
            if (tour == null)
                return NotFound();

            _context.Tours.Remove(tour);
            _context.SaveChanges();

            return RedirectToAction("Dashboard", "Agency"); // or Index or wherever you want
        }

        public IActionResult Details(int id)
{
    var tour = _context.Tours.FirstOrDefault(t => t.Id == id);
    if (tour == null)
    {
        return NotFound();
    }
    return View(tour);
}

    }
}
