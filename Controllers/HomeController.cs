using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TripNest.Models;
using TripNest.Data;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace TripNest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var tours = _context.Tours.Where(t => t.Status == "Active").Take(3).ToList();
                _logger.LogInformation($"Loaded {tours.Count} tours for Index page.");
                return View(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Index page.");
                return View("Error");
            }
        }

        public IActionResult Packages()
        {
            try
            {
                var tours = _context.Tours.Where(t => t.Status == "Active").ToList();
                _logger.LogInformation($"Loaded {tours.Count} tours for Packages page.");
                return View(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Packages page.");
                return View("Error");
            }
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}