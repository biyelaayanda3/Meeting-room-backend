namespace MeetingRoomBooking.Domain.Exceptions;

public class BookingConflictException : Exception
{
    public BookingConflictException()
           : base("This room is already booked for the requested time period") { }
    public BookingConflictException(string message)
        : base(message) { }
}
