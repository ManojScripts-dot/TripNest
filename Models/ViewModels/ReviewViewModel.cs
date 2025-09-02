using System;

namespace TripNest.Models.ViewModels
{
   public class ReviewViewModel
{
    public string TourTitle { get; set; } = string.Empty;
    public string TourDestination { get; set; } = string.Empty;  // Add this
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

}
