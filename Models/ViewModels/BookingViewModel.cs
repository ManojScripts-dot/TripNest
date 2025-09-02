public class BookingViewModel
{
    public int Id { get; set; }
    public DateTime BookingDate { get; set; }
    public required string Status { get; set; }
    public int AgencyId { get; set; }
    public required string TourName { get; set; }

    public int NumberOfGuests { get; set; }  // Add number of guests

    public string? TourImageUrl { get; set; } // URL to tour image

    public string? AgencyContactEmail { get; set; } // For contact agency button
}
