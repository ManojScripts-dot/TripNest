using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;
using TripNest.Services;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace TripNest.Controllers
{
    public class TourController : Controller
    {
    private readonly ApplicationDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly LocalImageService _localImageService;
    private readonly ILogger<TourController> _logger;

        public TourController(ApplicationDbContext context, ICloudinaryService cloudinaryService, LocalImageService localImageService, ILogger<TourController> logger)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _localImageService = localImageService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var tours = _context.Tours.ToList();
            return View(tours);
        }

        public IActionResult Create()
        {
            var agencyIdCookie = Request.Cookies["AgencyId"];
            if (string.IsNullOrEmpty(agencyIdCookie) || !int.TryParse(agencyIdCookie, out int agencyId))
            {
                return RedirectToAction("Login", "Agency");
            }

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
                    // Handle image upload - prioritize local storage for Docker deployment
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        try
                        {
                            _logger.LogInformation("Attempting to save image locally for tour: {TourTitle}, File size: {FileSize} bytes", tour.Title, imageFile.Length);
                            var localImageUrl = await _localImageService.SaveImageAsync(imageFile, "tour-images");
                            tour.ImagePath = localImageUrl;
                            _logger.LogInformation("Image saved locally for tour: {TourTitle}, URL: {ImageUrl}", tour.Title, localImageUrl);
                            ViewBag.Success = "Tour created successfully with uploaded image!";
                        }
                        catch (ArgumentException ex)
                        {
                            _logger.LogError(ex, "Invalid image file for tour: {TourTitle}", tour.Title);
                            ModelState.AddModelError("imageFile", ex.Message);
                            return View(tour);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error saving image locally for tour: {TourTitle}", tour.Title);
                            
                            // Try Cloudinary as fallback
                            try
                            {
                                _logger.LogInformation("Trying Cloudinary as fallback for tour: {TourTitle}", tour.Title);
                                var cloudinaryUrl = await _cloudinaryService.UploadImageAsync(imageFile, "tours");
                                
                                if (cloudinaryUrl.Contains("default-tour"))
                                {
                                    throw new Exception("Cloudinary returned default image");
                                }
                                
                                tour.ImagePath = cloudinaryUrl;
                                _logger.LogInformation("Image uploaded to Cloudinary for tour: {TourTitle}, URL: {ImageUrl}", tour.Title, cloudinaryUrl);
                                ViewBag.Success = "Tour created successfully with Cloudinary image!";
                            }
                            catch (Exception cloudinaryEx)
                            {
                                _logger.LogError(cloudinaryEx, "Cloudinary fallback also failed for tour: {TourTitle}", tour.Title);
                                // Use default image but don't fail the tour creation
                                tour.ImagePath = "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-tour_clzafv.jpg";
                                _logger.LogWarning("Using default image for tour: {TourTitle} due to upload failure", tour.Title);
                                ViewBag.Warning = "Image upload failed. Using default image.";
                            }
                        }
                    }
                    else
                    {
                        // Use default image URL
                        tour.ImagePath = "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-tour_clzafv.jpg";
                        _logger.LogInformation("No image provided for tour: {TourTitle}, using default image", tour.Title);
                        ViewBag.Success = "Tour created successfully with default image!";
                    }

                    _context.Tours.Add(tour);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Tour created successfully: {TourId} - {TourTitle}", tour.Id, tour.Title);
                    return RedirectToAction("Dashboard", "Agency");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving tour: {TourTitle}", tour.Title);
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

                    // Store old image path for potential cleanup
                    var oldImagePath = existingTour.ImagePath;

                    // Update tour properties
                    existingTour.Title = tour.Title;
                    existingTour.Description = tour.Description;
                    existingTour.DurationDays = tour.DurationDays;
                    existingTour.StarRating = tour.StarRating;
                    existingTour.Price = tour.Price;
                    existingTour.Destination = tour.Destination;
                    existingTour.Status = tour.Status;

                    // Handle new image upload - prioritize local storage
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        try
                        {
                            _logger.LogInformation("Attempting to save new image locally for tour: {TourTitle}, File size: {FileSize} bytes", existingTour.Title, imageFile.Length);
                            var localImageUrl = await _localImageService.SaveImageAsync(imageFile, "tour-images");
                            
                            // Delete old local image if it exists
                            if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath.StartsWith("/"))
                            {
                                _localImageService.DeleteImage(oldImagePath);
                                _logger.LogInformation("Old local image deleted: {OldImagePath}", oldImagePath);
                            }
                            
                            existingTour.ImagePath = localImageUrl;
                            _logger.LogInformation("Image updated locally for tour: {TourTitle}, New URL: {ImageUrl}", existingTour.Title, localImageUrl);
                        }
                        catch (ArgumentException ex)
                        {
                            ModelState.AddModelError("imageFile", ex.Message);
                            return View(tour);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error saving new image locally for tour: {TourTitle}", existingTour.Title);
                            
                            // Try Cloudinary as fallback
                            try
                            {
                                _logger.LogInformation("Trying Cloudinary as fallback for tour: {TourTitle}", existingTour.Title);
                                var cloudinaryUrl = await _cloudinaryService.UploadImageAsync(imageFile, "tours");
                                
                                if (!cloudinaryUrl.Contains("default-tour"))
                                {
                                    existingTour.ImagePath = cloudinaryUrl;
                                    _logger.LogInformation("Image updated via Cloudinary for tour: {TourTitle}, New URL: {ImageUrl}", existingTour.Title, cloudinaryUrl);
                                }
                                else
                                {
                                    throw new Exception("Cloudinary returned default image");
                                }
                            }
                            catch (Exception cloudinaryEx)
                            {
                                _logger.LogError(cloudinaryEx, "Cloudinary fallback also failed for tour: {TourTitle}", existingTour.Title);
                                _logger.LogWarning("Keeping existing image for tour: {TourTitle} due to upload failure", existingTour.Title);
                            }
                        }
                    }

                    _context.Tours.Update(existingTour);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Tour updated successfully: {TourId} - {TourTitle}", existingTour.Id, existingTour.Title);
                    return RedirectToAction("Dashboard", "Agency");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating tour: {TourId}", id);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tour = _context.Tours.Find(id);
            if (tour == null)
                return NotFound();

            try
            {
                // Delete image from Cloudinary if it exists and is not the default
                if (!string.IsNullOrEmpty(tour.ImagePath) && 
                    !tour.ImagePath.Contains("default-tour") &&
                    tour.ImagePath.Contains("cloudinary.com"))
                {
                    var publicId = _cloudinaryService.ExtractPublicIdFromUrl(tour.ImagePath);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        await _cloudinaryService.DeleteImageAsync(publicId);
                        _logger.LogInformation("Image deleted from Cloudinary: {PublicId}", publicId);
                    }
                }

                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Tour deleted successfully: {TourId} - {TourTitle}", tour.Id, tour.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tour: {TourId}", id);
                // Continue with tour deletion even if image deletion fails
                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }

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
                tours = tours.Where(t => t.Destination.ToLower().Contains(destination.ToLower()));
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