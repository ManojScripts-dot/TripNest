using System.ComponentModel.DataAnnotations;

namespace TripNest.Models.ViewModels
{
    public class RegisterViewModel
{
    [Required]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? ProfileImageUrl { get; set; }
}
}