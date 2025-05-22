using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripNest.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }  // Foreign key to User

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(50)]
        [Display(Name = "Preferred Language")]
        public string? PreferredLanguage { get; set; }

        [StringLength(250)]
        [Display(Name = "Profile Image URL")]
        public string? ProfileImageUrl { get; set; }
    }
}
