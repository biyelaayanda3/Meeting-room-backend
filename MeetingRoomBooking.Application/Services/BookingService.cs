using MeetingRoomBooking.Application.DTOs;
using MeetingRoomBooking.Application.Interfaces;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Domain.Enums;
using MeetingRoomBooking.Domain.Exceptions;
using Microsoft.Extensions.Logging;
namespace MeetingRoomBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _logger = logger;
    }

    public async Task<List<BookingResponseDto>>GetAllBookingsAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        return bookings.Select(MapToDto).ToList();
    }

    public async Task<List<BookingResponseDto>>GetBookingsByRoomAsync(int roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new NotFoundException("Room", roomId);

        var bookings = await _bookingRepository.GetByRoomIdAsync(roomId);
        return bookings.Select(MapToDto).ToList();
    }

    public async Task<BookingResponseDto>GetBookingByIdAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            throw new NotFoundException("Booking", id);

        return MapToDto(booking);
    }

    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
    {
        var room = await _roomRepository.GetByIdAsync(dto.RoomId);
        if (room == null)
            throw new NotFoundException("Room", dto.RoomId);

        var hasConflict = await _bookingRepository.HasConflictAsync(dto.RoomId, dto.StartTime, dto.EndTime);

        if (hasConflict)
            throw new BookingConflictException();

        var booking = new Booking
        {
            Title = dto.Title,
            OrganizerName = dto.OrganizerName,
            OrganizerEmail = dto.OrganizerEmail,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            RoomId = dto.RoomId,
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _bookingRepository
            .CreateAsync(booking);

        _logger.LogInformation("Booking created with ID {Id} for room {RoomId}",created.Id, created.RoomId);

        return MapToDto(created);
    }

    public async Task<BookingResponseDto> UpdateBookingAsync(int id, UpdateBookingDto dto)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            throw new NotFoundException("Booking", id);

        if (booking.Status == BookingStatus.Cancelled)
            throw new ValidationException("Cannot edit a cancelled booking");

        if (dto.RoomId != booking.RoomId)
        {
            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null)
                throw new NotFoundException("Room", dto.RoomId);
        }

        // Check conflicts — EXCLUDE current booking!
        var hasConflict = await _bookingRepository.HasConflictAsync(
                dto.RoomId, dto.StartTime, dto.EndTime, excludeBookingId: id);

        if (hasConflict)
            throw new BookingConflictException();

        // Update!
        booking.Title = dto.Title;
        booking.OrganizerName = dto.OrganizerName;
        booking.OrganizerEmail = dto.OrganizerEmail;
        booking.StartTime = dto.StartTime;
        booking.EndTime = dto.EndTime;
        booking.RoomId = dto.RoomId;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);

        _logger.LogInformation("Booking {Id} updated", id);

        return MapToDto(booking);
    }

    public async Task CancelBookingAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
            throw new NotFoundException("Booking", id);

        if (booking.Status == BookingStatus.Cancelled)
            throw new ValidationException("Booking is already cancelled");

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);

        _logger.LogInformation("Booking {Id} cancelled", id);
    }

    private static BookingResponseDto MapToDto(Booking booking) => new()
    {
        Id = booking.Id,
        Title = booking.Title,
        OrganizerName = booking.OrganizerName,
        OrganizerEmail = booking.OrganizerEmail,
        StartTime = booking.StartTime,
        EndTime = booking.EndTime,
        Status = booking.Status.ToString(),
        CreatedAt = booking.CreatedAt,
        UpdatedAt = booking.UpdatedAt,
        RoomId = booking.RoomId,
        RoomName = booking.Room?.Name ?? string.Empty
    };
}