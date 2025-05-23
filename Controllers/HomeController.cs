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
            var tours = _context.Tours.Take(3).ToList(); // Limit to top 3 tours for Featured Tours
            return View(tours);
        }

        public IActionResult Packages()
        {
            var tours = _context.Tours.ToList(); // Fetch all tours for Packages page
            return View(tours);
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