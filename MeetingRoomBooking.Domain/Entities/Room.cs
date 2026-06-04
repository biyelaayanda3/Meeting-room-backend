namespace MeetingRoomBooking.Domain.Entities;
public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<Booking> Bookings { get; set; }
        = new List<Booking>();
}
