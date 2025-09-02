using System.Collections.Generic;

namespace TripNest.Models.ViewModels
{
    public class ProfileDashboardViewModel
    {
        public required RegisterViewModel Profile { get; set; }
        public required List<BookingViewModel> Bookings { get; set; }
        public required List<ReviewViewModel> Reviews { get; set; }
    }
}
