namespace MeetingRoomBooking.Application.DTOs;

public class BookingResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OrganizerName { get; set; } = string.Empty;
    public string OrganizerEmail { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Include room info in response!
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
}

public class CreateBookingDto
{
    public string Title { get; set; } = string.Empty;
    public string OrganizerName { get; set; } = string.Empty;
    public string OrganizerEmail { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RoomId { get; set; }
}

public class UpdateBookingDto
{
    public string Title { get; set; } = string.Empty;
    public string OrganizerName { get; set; } = string.Empty;
    public string OrganizerEmail { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RoomId { get; set; }
}