
using MeetingRoomBooking.Domain.Entities;
namespace MeetingRoomBooking.Application.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetByRoomIdAsync(int roomId);
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking> CreateAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task<bool> HasConflictAsync(int roomId, DateTime startTime,DateTime endTime, int? excludeBookingId = null);
}