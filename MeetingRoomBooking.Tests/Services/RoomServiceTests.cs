using FluentAssertions;
using MeetingRoomBooking.Application.Interfaces;
using Xunit;
using MeetingRoomBooking.Application.Services;
using MeetingRoomBooking.Domain.Entities;
using MeetingRoomBooking.Domain.Exceptions;
using Moq;

namespace MeetingRoomBooking.Tests.Services;

public class RoomServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepo = new();
    private readonly RoomService _sut;

    public RoomServiceTests()
    {
        _sut = new RoomService(_roomRepo.Object);
    }

    [Fact]
    public async Task GetAllRooms_WhenRoomsExist_ReturnsAllRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            CreateRoom(1, "Board Room", capacity: 20),
            CreateRoom(2, "Focus Room A", capacity: 4)
        };
        _roomRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);

        // Act
        var result = await _sut.GetAllRoomsAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Board Room");
        result[1].Name.Should().Be("Focus Room A");
    }

    [Fact]
    public async Task GetAllRooms_WhenNoRoomsExist_ReturnsEmptyList()
    {
        // Arrange
        _roomRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Room>());

        // Act
        var result = await _sut.GetAllRoomsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllRooms_MapsPropertiesCorrectly()
    {
        // Arrange
        var rooms = new List<Room>
        {
            CreateRoom(1, "Board Room", description: "Main boardroom", capacity: 20)
        };
        _roomRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);

        // Act
        var result = await _sut.GetAllRoomsAsync();

        // Assert
        var room = result.Single();
        room.Id.Should().Be(1);
        room.Name.Should().Be("Board Room");
        room.Description.Should().Be("Main boardroom");
        room.Capacity.Should().Be(20);
    }

    [Fact]
    public async Task GetRoomById_WhenRoomExists_ReturnsCorrectRoom()
    {
        // Arrange
        var room = CreateRoom(1, "Board Room", capacity: 20);
        _roomRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);

        // Act
        var result = await _sut.GetRoomByIdAsync(1);

        // Assert
        result.Id.Should().Be(1);
        result.Name.Should().Be("Board Room");
        result.Capacity.Should().Be(20);
    }

    [Fact]
    public async Task GetRoomById_WhenRoomDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _roomRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Room?)null);

        // Act
        var act = async () => await _sut.GetRoomByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetRoomById_WhenRoomDoesNotExist_ExceptionMentionsRoomId()
    {
        // Arrange
        _roomRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Room?)null);

        // Act
        var act = async () => await _sut.GetRoomByIdAsync(5);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*5*");
    }

    private static Room CreateRoom(int id, string name, string description = "A meeting room", int capacity = 10) => new()
    {
        Id = id,
        Name = name,
        Description = description,
        Capacity = capacity,
        IsActive = true
    };
}
