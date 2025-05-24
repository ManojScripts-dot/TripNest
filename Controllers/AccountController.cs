using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TripNest.Data;
using TripNest.Models;
using TripNest.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TripNest.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View("UserRegister");
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    Password = model.Password,
                    UserProfile = new UserProfile
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        Country = model.Country,
                        PreferredLanguage = model.PreferredLanguage,
                        ProfileImageUrl = model.ProfileImageUrl
                    }
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View("UserRegister", model);
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("UserLogin");
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("FirstName", user.UserProfile?.FirstName ?? "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View("UserLogin");
        }

        // GET: /Account/Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: /Account/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userEmail = User.Identity?.Name;

            var userEntity = await _context.Users
                .Include(u => u.UserProfile)
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Tour)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (userEntity == null || userEntity.UserProfile == null)
            {
                return RedirectToAction("Login");
            }

            var userReviews = await _context.Reviews
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Tour)
                .Where(r => r.UserId == userEntity.Id)
                .Select(r => new ReviewViewModel
                {
                    TourTitle = r.Booking != null && r.Booking.Tour != null ? r.Booking.Tour.Title : "Unknown Tour",
                    Content = r.Message ?? string.Empty,
                    Rating = r.Stars,
                    CreatedAt = r.CreatedAt
                }).ToListAsync();

            var model = new ProfileDashboardViewModel
            {
                Profile = new RegisterViewModel
                {
                    Email = userEntity.Email,
                    Password = string.Empty,
                    FirstName = userEntity.UserProfile.FirstName,
                    LastName = userEntity.UserProfile.LastName,
                    PhoneNumber = userEntity.UserProfile.PhoneNumber,
                    Address = userEntity.UserProfile.Address,
                    Country = userEntity.UserProfile.Country,
                    PreferredLanguage = userEntity.UserProfile.PreferredLanguage,
                    ProfileImageUrl = userEntity.UserProfile.ProfileImageUrl
                },
                Bookings = userEntity.Bookings.Select(b => new BookingViewModel
                {
                    Id = b.Id,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    TourName = b.Tour != null ? b.Tour.Title : "Unknown"
                }).ToList(),
                Reviews = userReviews
            };

            return View("ProfileDashboard", model);
        }

        // POST: /Account/Profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(RegisterViewModel model)
        {
            var userEmail = User.Identity?.Name;

            if (ModelState.IsValid)
            {
                var userEntity = await _context.Users
                    .Include(u => u.UserProfile)
                    .Include(u => u.Bookings)
                        .ThenInclude(b => b.Tour)
                    .FirstOrDefaultAsync(u => u.Email == userEmail);

                if (userEntity == null || userEntity.UserProfile == null)
                {
                    return RedirectToAction("Login");
                }

                userEntity.UserProfile.FirstName = model.FirstName;
                userEntity.UserProfile.LastName = model.LastName;
                userEntity.UserProfile.PhoneNumber = model.PhoneNumber;
                userEntity.UserProfile.Address = model.Address;
                userEntity.UserProfile.Country = model.Country;
                userEntity.UserProfile.PreferredLanguage = model.PreferredLanguage;
                userEntity.UserProfile.ProfileImageUrl = model.ProfileImageUrl;

                await _context.SaveChangesAsync();

                // Update the claims with the new first name
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userEntity.Email),
                    new Claim("UserId", userEntity.Id.ToString()),
                    new Claim("FirstName", userEntity.UserProfile.FirstName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                ViewBag.Message = "Profile updated successfully!";

                var userReviews = await _context.Reviews
                    .Include(r => r.Booking)
                        .ThenInclude(b => b.Tour)
                    .Where(r => r.UserId == userEntity.Id)
                    .Select(r => new ReviewViewModel
                    {
                        TourTitle = r.Booking != null && r.Booking.Tour != null ? r.Booking.Tour.Title : "Unknown Tour",
                        Content = r.Message ?? string.Empty,
                        Rating = r.Stars,
                        CreatedAt = r.CreatedAt
                    }).ToListAsync();

                var updatedModel = new ProfileDashboardViewModel
                {
                    Profile = model,
                    Bookings = userEntity.Bookings.Select(b => new BookingViewModel
                    {
                        Id = b.Id,
                        BookingDate = b.BookingDate,
                        Status = b.Status,
                        TourName = b.Tour != null ? b.Tour.Title : "Unknown"
                    }).ToList(),
                    Reviews = userReviews
                };

                return View("ProfileDashboard", updatedModel);
            }

            var fallbackReviews = await _context.Users
                .Where(u => u.Email == userEmail)
                .SelectMany(u => _context.Reviews
                    .Include(r => r.Booking)
                        .ThenInclude(b => b.Tour)
                    .Where(r => r.UserId == u.Id)
                    .Select(r => new ReviewViewModel
                    {
                        TourTitle = r.Booking != null && r.Booking.Tour != null ? r.Booking.Tour.Title : "Unknown Tour",
                        Content = r.Message ?? string.Empty,
                        Rating = r.Stars,
                        CreatedAt = r.CreatedAt
                    })).ToListAsync();

            return View("ProfileDashboard", new ProfileDashboardViewModel
            {
                Profile = model,
                Bookings = new List<BookingViewModel>(),
                Reviews = fallbackReviews
            });
        }
    }
}