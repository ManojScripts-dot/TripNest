using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace TripNest.Controllers
{
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
        public async Task<IActionResult> Create(Tour tour, IFormFile imageFile)
        {
            var agencyIdCookie = Request.Cookies["AgencyId"];
            if (string.IsNullOrEmpty(agencyIdCookie) || !int.TryParse(agencyIdCookie, out int agencyId))
            {
                ModelState.AddModelError("", "You must be logged in as an agency.");
                return View(tour);
            }

            tour.AgencyId = agencyId;

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (imageFile.Length > 10 * 1024 * 1024)
                        {
                            ModelState.AddModelError("imageFile", "Image file size must be less than 10MB.");
                            return View(tour);
                        }
                        if (!imageFile.ContentType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("imageFile", "Please upload a valid image file (e.g., .jpg, .png).");
                            return View(tour);
                        }

                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        tour.ImagePath = $"images/{fileName}";
                    }
                    else
                    {
                        tour.ImagePath = "images/default-tour.jpg";
                    }

                    _context.Tours.Add(tour);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Dashboard", "Agency");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while saving the tour: {ex.Message}");
                    return View(tour);
                }
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
        public async Task<IActionResult> Edit(int id, Tour tour, IFormFile imageFile)
        {
            if (id != tour.Id)
                return BadRequest();

            var agencyIdCookie = Request.Cookies["AgencyId"];
            if (string.IsNullOrEmpty(agencyIdCookie) || !int.TryParse(agencyIdCookie, out int agencyId))
            {
                return RedirectToAction("Login", "Agency");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTour = _context.Tours.Find(id);
                    if (existingTour == null)
                        return NotFound();

                    existingTour.Title = tour.Title;
                    existingTour.Description = tour.Description;
                    existingTour.DurationDays = tour.DurationDays;
                    existingTour.StarRating = tour.StarRating;
                    existingTour.Price = tour.Price;
                    existingTour.Destination = tour.Destination;
                    existingTour.Status = tour.Status;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (imageFile.Length > 10 * 1024 * 1024)
                        {
                            ModelState.AddModelError("imageFile", "Image file size must be less than 10MB.");
                            return View(tour);
                        }
                        if (!imageFile.ContentType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("imageFile", "Please upload a valid image file (e.g., .jpg, .png).");
                            return View(tour);
                        }

                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        existingTour.ImagePath = $"images/{fileName}";
                    }

                    _context.Tours.Update(existingTour);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Dashboard", "Agency");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while updating the tour: {ex.Message}");
                    return View(tour);
                }
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
                tours = tours.Where(t => t.Destination.Contains(destination) || t.Title.Contains(destination));
            }

            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, out var parsedDate))
                {
                    tours = tours.Where(t => t.CreatedDate.Date >= parsedDate.Date);
                }
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                var range = priceRange.Split('-');
                if (range.Length == 2 && decimal.TryParse(range[0], out var minPrice))
                {
                    if (range[1] == "+")
                    {
                        tours = tours.Where(t => t.Price >= minPrice);
                    }
                    else if (decimal.TryParse(range[1], out var maxPrice))
                    {
                        tours = tours.Where(t => t.Price >= minPrice && t.Price <= maxPrice);
                    }
                }
            }

            return View("Index", tours.ToList());
        }
    }
}