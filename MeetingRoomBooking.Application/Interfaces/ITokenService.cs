using MeetingRoomBooking.Domain.Entities;
namespace MeetingRoomBooking.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}