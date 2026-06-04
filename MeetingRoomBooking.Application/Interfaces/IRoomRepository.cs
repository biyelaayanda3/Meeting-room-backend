using MeetingRoomBooking.Domain.Entities;
namespace MeetingRoomBooking.Application.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(int id);
}