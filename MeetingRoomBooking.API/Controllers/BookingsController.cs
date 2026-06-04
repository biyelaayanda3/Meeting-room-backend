using Microsoft.AspNetCore.Mvc;
using MeetingRoomBooking.Application.DTOs;
using MeetingRoomBooking.Application.Interfaces;

namespace MeetingRoomBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        _logger.LogInformation("Getting all bookings");
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("room/{roomId}")]
    public async Task<IActionResult> GetBookingsByRoom(int roomId)
    {
        _logger.LogInformation("Getting bookings for room {RoomId}", roomId);
        var bookings = await _bookingService.GetBookingsByRoomAsync(roomId);
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        _logger.LogInformation("Getting booking {Id}", id);
        var booking = await _bookingService.GetBookingByIdAsync(id);
        return Ok(booking);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        _logger.LogInformation("Creating booking for room {RoomId}", dto.RoomId);
        var booking = await _bookingService.CreateBookingAsync(dto);
        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto dto)
    {
        _logger.LogInformation("Updating booking {Id}", id);
        var booking = await _bookingService.UpdateBookingAsync(id, dto);
        return Ok(booking);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CancelBooking(int id)
    {
        _logger.LogInformation("Cancelling booking {Id}", id);
        await _bookingService.CancelBookingAsync(id);
        return NoContent();
    }
}