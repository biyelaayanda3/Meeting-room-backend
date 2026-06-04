using MeetingRoomBooking.Application.DTOs;
using MeetingRoomBooking.Application.Interfaces;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Domain.Exceptions;
namespace MeetingRoomBooking.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    public async Task<List<RoomResponseDto>> GetAllRoomsAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return rooms.Select(MapToDto).ToList();
    }

    public async Task<RoomResponseDto> GetRoomByIdAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
            throw new NotFoundException("Room", id);

        return MapToDto(room);
    }

    private static RoomResponseDto MapToDto(Room room) => new()
        {
            Id = room.Id,
            Name = room.Name,
            Description = room.Description,
            Capacity = room.Capacity
        };
}