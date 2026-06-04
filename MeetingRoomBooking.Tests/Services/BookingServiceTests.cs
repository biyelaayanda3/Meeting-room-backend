using FluentAssertions;
using MeetingRoomBooking.Application.DTOs;
using Xunit;
using MeetingRoomBooking.Application.Interfaces;
using MeetingRoomBooking.Application.Services;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Domain.Enums;
using MeetingRoomBooking.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeetingRoomBooking.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepo = new();
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly Mock<ILogger<BookingService>> _logger = new();
    private readonly BookingService _sut;

    public BookingServiceTests()
    {
        _sut = new BookingService(_bookingRepo.Object, _roomRepo.Object, _logger.Object);
    }

    [Fact]
    public async Task GetAllBookings_WhenBookingsExist_ReturnsAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            CreateBooking(1, "Team Standup"),
            CreateBooking(2, "Sprint Review")
        };
        _bookingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(bookings);

        // Act
        var result = await _sut.GetAllBookingsAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Team Standup");
        result[1].Title.Should().Be("Sprint Review");
    }

    [Fact]
    public async Task GetAllBookings_WhenNoBookingsExist_ReturnsEmptyList()
    {
        // Arrange
        _bookingRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _sut.GetAllBookingsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBookingById_WhenBookingExists_ReturnsCorrectBooking()
    {
        // Arrange
        var booking = CreateBooking(1, "Board Meeting");
        _bookingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);

        // Act
        var result = await _sut.GetBookingByIdAsync(1);

        // Assert
        result.Id.Should().Be(1);
        result.Title.Should().Be("Board Meeting");
    }

    [Fact]
    public async Task GetBookingById_WhenBookingDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _bookingRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Booking?)null);

        // Act
        var act = async () => await _sut.GetBookingByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetBookingsByRoom_WhenRoomDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _roomRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Room?)null);

        // Act
        var act = async () => await _sut.GetBookingsByRoomAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetBookingsByRoom_WhenRoomExists_ReturnsItsBookings()
    {
        // Arrange
        var room = CreateRoom(1, "Board Room");
        var bookings = new List<Booking> { CreateBooking(1, "Team Standup") };

        _roomRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);
        _bookingRepo.Setup(r => r.GetByRoomIdAsync(1)).ReturnsAsync(bookings);

        // Act
        var result = await _sut.GetBookingsByRoomAsync(1);

        // Assert
        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Team Standup");
    }

    [Fact]
    public async Task CreateBooking_WhenEverythingIsValid_ReturnsCreatedBooking()
    {
        // Arrange
        var room = CreateRoom(1, "Board Room");
        var dto = BuildCreateDto(roomId: 1);
        var savedBooking = CreateBooking(10, dto.Title, roomId: 1);

        _roomRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);
        _bookingRepo.Setup(r => r.HasConflictAsync(1, dto.StartTime, dto.EndTime, null)).ReturnsAsync(false);
        _bookingRepo.Setup(r => r.CreateAsync(It.IsAny<Booking>())).ReturnsAsync(savedBooking);

        // Act
        var result = await _sut.CreateBookingAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
    }

    [Fact]
    public async Task CreateBooking_WhenRoomDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _roomRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Room?)null);
        var dto = BuildCreateDto(roomId: 99);

        // Act
        var act = async () => await _sut.CreateBookingAsync(dto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateBooking_WhenRoomHasConflict_ThrowsBookingConflictException()
    {
        // Arrange
        var room = CreateRoom(1, "Board Room");
        var dto = BuildCreateDto(roomId: 1);

        _roomRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);
        _bookingRepo.Setup(r => r.HasConflictAsync(1, dto.StartTime, dto.EndTime, null)).ReturnsAsync(true);

        // Act
        var act = async () => await _sut.CreateBookingAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BookingConflictException>();
    }

    [Fact]
    public async Task CancelBooking_WhenBookingExists_CancelsSuccessfully()
    {
        // Arrange
        var booking = CreateBooking(1, "Team Standup");
        _bookingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);
        _bookingRepo.Setup(r => r.UpdateAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);

        // Act
        await _sut.CancelBookingAsync(1);

        // Assert
        _bookingRepo.Verify(r => r.UpdateAsync(It.Is<Booking>(b => b.Status == BookingStatus.Cancelled)), Times.Once);
    }

    [Fact]
    public async Task CancelBooking_WhenBookingDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _bookingRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Booking?)null);

        // Act
        var act = async () => await _sut.CancelBookingAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CancelBooking_WhenAlreadyCancelled_ThrowsValidationException()
    {
        // Arrange
        var booking = CreateBooking(1, "Team Standup");
        booking.Status = BookingStatus.Cancelled;
        _bookingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);

        // Act
        var act = async () => await _sut.CancelBookingAsync(1);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*already cancelled*");
    }

    [Fact]
    public async Task UpdateBooking_WhenBookingDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _bookingRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Booking?)null);

        // Act
        var act = async () => await _sut.UpdateBookingAsync(99, new UpdateBookingDto());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateBooking_WhenBookingIsCancelled_ThrowsValidationException()
    {
        // Arrange
        var booking = CreateBooking(1, "Team Standup");
        booking.Status = BookingStatus.Cancelled;
        _bookingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);

        // Act
        var act = async () => await _sut.UpdateBookingAsync(1, new UpdateBookingDto { RoomId = 1 });

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*cancelled booking*");
    }

    [Fact]
    public async Task UpdateBooking_WhenNewRoomHasConflict_ThrowsBookingConflictException()
    {
        // Arrange
        var booking = CreateBooking(1, "Team Standup", roomId: 1);
        var dto = BuildUpdateDto(roomId: 1);

        _bookingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);
        _bookingRepo.Setup(r => r.HasConflictAsync(1, dto.StartTime, dto.EndTime, 1)).ReturnsAsync(true);

        // Act
        var act = async () => await _sut.UpdateBookingAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<BookingConflictException>();
    }

    private static Room CreateRoom(int id, string name) => new()
    {
        Id = id,
        Name = name,
        IsActive = true
    };

    private static Booking CreateBooking(int id, string title, int roomId = 1) => new()
    {
        Id = id,
        Title = title,
        OrganizerName = "Test User",
        OrganizerEmail = "test@test.com",
        StartTime = DateTime.UtcNow.AddDays(1),
        EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
        Status = BookingStatus.Active,
        RoomId = roomId,
        Room = CreateRoom(roomId, "Test Room"),
        CreatedAt = DateTime.UtcNow
    };

    private static CreateBookingDto BuildCreateDto(int roomId = 1) => new()
    {
        Title = "Team Meeting",
        OrganizerName = "Jane Smith",
        OrganizerEmail = "jane@company.com",
        StartTime = DateTime.UtcNow.AddDays(1),
        EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
        RoomId = roomId
    };

    private static UpdateBookingDto BuildUpdateDto(int roomId = 1) => new()
    {
        Title = "Updated Meeting",
        OrganizerName = "Jane Smith",
        OrganizerEmail = "jane@company.com",
        StartTime = DateTime.UtcNow.AddDays(2),
        EndTime = DateTime.UtcNow.AddDays(2).AddHours(1),
        RoomId = roomId
    };
}
