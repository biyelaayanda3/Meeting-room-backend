using MeetingRoomBooking.Application.DTOs;
namespace MeetingRoomBooking.Application.Interfaces;

public interface IBookingService
{
    Task<List<BookingResponseDto>> GetAllBookingsAsync();
    Task<List<BookingResponseDto>> GetBookingsByRoomAsync(int roomId);
    Task<BookingResponseDto> GetBookingByIdAsync(int id);
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto);
    Task<BookingResponseDto> UpdateBookingAsync(int id, UpdateBookingDto dto);
    Task CancelBookingAsync(int id);
}
