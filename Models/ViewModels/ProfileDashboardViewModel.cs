using System;
using System.Collections.Generic;

namespace TripNest.Models.ViewModels
{
    public class ProfileDashboardViewModel
{
    public required RegisterViewModel Profile { get; set; }
    public required List<BookingViewModel> Bookings { get; set; }
}

}
