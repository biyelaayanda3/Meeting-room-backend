
namespace MeetingRoomBooking.Domain.Exceptions;
public class NotFoundException : Exception
{
    public NotFoundException(string entity, int id)
            : base($"{entity} with ID {id} was not found") { }

    public NotFoundException(string message)
        : base(message) { }
}
