using Microsoft.AspNetCore.Mvc;

namespace TripNest.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserLogin(UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Authentication logic here

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AgencyLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AgencyLogin(AgencyLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Authentication logic here

            return RedirectToAction("AgencyDashboard", "Agency");
        }
    }
}
