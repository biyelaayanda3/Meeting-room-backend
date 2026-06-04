using MeetingRoomBooking.Application.Interfaces;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Domain.Enums;
using MeetingRoomBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBooking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Room) // Include room name!
            .Where(b => b.Status == BookingStatus.Active)
            .OrderBy(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByRoomIdAsync(int roomId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Room)
            .Where(b => b.RoomId == roomId
                && b.Status == BookingStatus.Active)
            .OrderBy(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Reload with Room included!
        return await GetByIdAsync(booking.Id) ?? booking;
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    // THE MOST IMPORTANT METHOD — Conflict Detection!
    public async Task<bool> HasConflictAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
    {
        var query = _context.Bookings
            .Where(b =>
                b.RoomId == roomId && b.Status == BookingStatus.Active
                // Overlap condition!
                // NOT (new ends before existing starts
                //      OR new starts after existing ends)
                && !(endTime <= b.StartTime || startTime >= b.EndTime));

        // When editing — exclude the booking being edited!
        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        return await query.AnyAsync();
    }
}