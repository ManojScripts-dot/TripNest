using Microsoft.AspNetCore.Mvc;
using TripNest.Data;
using TripNest.Models;

namespace TripNest.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Submit([FromBody] ContactMessage contact)
        {
            Console.WriteLine($"Received JSON: {System.Text.Json.JsonSerializer.Serialize(contact)}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(string.Join("; ", errors));
            }

            _context.ContactMessages.Add(contact);
            await _context.SaveChangesAsync();

            return Ok("Thank you for your message!");
        }


    }
}
