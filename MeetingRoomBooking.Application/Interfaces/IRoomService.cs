using MeetingRoomBooking.Application.DTOs;
namespace MeetingRoomBooking.Application.Interfaces;

public interface IRoomService
{
    Task<List<RoomResponseDto>> GetAllRoomsAsync();
    Task<RoomResponseDto> GetRoomByIdAsync(int id);
}