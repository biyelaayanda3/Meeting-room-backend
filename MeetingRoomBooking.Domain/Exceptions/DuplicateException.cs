namespace MeetingRoomBooking.Domain.Exceptions;

public class DuplicateException : Exception
{
    public DuplicateException(string entity, string field)
         : base($"{entity} with that {field} already exists") { }
}
