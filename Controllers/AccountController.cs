using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
    var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
    if (user != null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            
            new Claim("UserId", user.Id.ToString())  // Added claim here
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
        public IActionResult Profile()
        {
            var userEmail = User.Identity?.Name;

            var userEntity = _context.Users
                .Include(u => u.UserProfile)
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Tour)
                .FirstOrDefault(u => u.Email == userEmail);

            if (userEntity == null || userEntity.UserProfile == null)
            {
                return RedirectToAction("Login");
            }

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
                    TourName = b.Tour?.Title ?? "Unknown"
                }).ToList()
            };

            return View("ProfileDashboard", model);
        }

        // POST: /Account/Profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(RegisterViewModel model)
        {
            var userEmail = User.Identity?.Name;

            if (ModelState.IsValid)
            {
                var userEntity = _context.Users
                    .Include(u => u.UserProfile)
                    .Include(u => u.Bookings)
                        .ThenInclude(b => b.Tour)
                    .FirstOrDefault(u => u.Email == userEmail);

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

                _context.SaveChanges();

                ViewBag.Message = "Profile updated successfully!";

                var updatedModel = new ProfileDashboardViewModel
                {
                    Profile = model,
                    Bookings = userEntity.Bookings.Select(b => new BookingViewModel
                    {
                        Id = b.Id,
                        BookingDate = b.BookingDate,
                        Status = b.Status,
                        TourName = b.Tour?.Title ?? "Unknown"
                    }).ToList()
                };

                return View("ProfileDashboard", updatedModel);
            }

            return View("ProfileDashboard", new ProfileDashboardViewModel
            {
                Profile = model,
                Bookings = new List<BookingViewModel>()
            });
        }
    }
}
