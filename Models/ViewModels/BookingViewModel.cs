using System;

namespace TripNest.Models.ViewModels
{
    public class BookingViewModel
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }
    public required string Status { get; set; }
    public int AgencyId { get; set; }
    public required string TourName { get; set; }
}
}
