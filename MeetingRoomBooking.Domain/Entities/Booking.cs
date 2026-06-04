using MeetingRoomBooking.Domain.Enums;
namespace MeetingRoomBooking.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OrganizerName { get; set; } = string.Empty;
    public string OrganizerEmail { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Active;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Foreign key
    public int RoomId { get; set; }

    // Navigation property
    public Room Room { get; set; } = null!;
}
