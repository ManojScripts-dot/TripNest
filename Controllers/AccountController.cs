using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TripNest.Data;
using TripNest.Models;
using TripNest.Models.ViewModels;
using TripNest.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TripNest.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ICloudinaryService cloudinaryService, ILogger<AccountController> logger)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle profile image upload if provided
                    string profileImageUrl = null;
                    if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                    {
                        try
                        {
                            profileImageUrl = await _cloudinaryService.UploadImageAsync(model.ProfileImage, "profiles");
                            _logger.LogInformation("Profile image uploaded successfully for user: {Email}", model.Email);
                        }
                        catch (ArgumentException ex)
                        {
                            ModelState.AddModelError("ProfileImage", ex.Message);
                            return View("UserRegister", model);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error uploading profile image for user: {Email}", model.Email);
                            // Don't fail registration for image upload issues, use default instead
                            profileImageUrl = "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-profile_clzafv.jpg";
                        }
                    }
                    else
                    {
                        // Use default profile image
                        profileImageUrl = "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-profile_clzafv.jpg";
                    }

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
                            ProfileImageUrl = profileImageUrl
                        }
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User registered successfully: {Email}", model.Email);
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during user registration: {Email}", model.Email);
                    ModelState.AddModelError("", "An error occurred while creating your account. Please try again.");
                    return View("UserRegister", model);
                }
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

                // Handle profile image update if new image is provided
                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    try
                    {
                        var oldImagePath = userEntity.UserProfile.ProfileImageUrl;
                        var newImageUrl = await _cloudinaryService.UploadImageAsync(model.ProfileImage, "profiles");
                        
                        userEntity.UserProfile.ProfileImageUrl = newImageUrl;

                        // Delete old image if it's not the default
                        if (!string.IsNullOrEmpty(oldImagePath) && 
                            !oldImagePath.Contains("default-profile") &&
                            oldImagePath.Contains("cloudinary.com"))
                        {
                            var publicId = _cloudinaryService.ExtractPublicIdFromUrl(oldImagePath);
                            if (!string.IsNullOrEmpty(publicId))
                            {
                                await _cloudinaryService.DeleteImageAsync(publicId);
                                _logger.LogInformation("Old profile image deleted: {PublicId}", publicId);
                            }
                        }

                        _logger.LogInformation("Profile image updated successfully for user: {Email}", userEmail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating profile image for user: {Email}", userEmail);
                        ModelState.AddModelError("ProfileImage", "Failed to update profile image. Please try again.");
                        // Don't return here, continue with other profile updates
                    }
                }

                userEntity.UserProfile.FirstName = model.FirstName;
                userEntity.UserProfile.LastName = model.LastName;
                userEntity.UserProfile.PhoneNumber = model.PhoneNumber;
                userEntity.UserProfile.Address = model.Address;
                userEntity.UserProfile.Country = model.Country;
                userEntity.UserProfile.PreferredLanguage = model.PreferredLanguage;

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