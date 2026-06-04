using FluentAssertions;
using MeetingRoomBooking.Application.DTOs;
using MeetingRoomBooking.Application.Validators;
using Xunit;

namespace MeetingRoomBooking.Tests.Validators;

public class UpdateBookingValidatorTests
{
    private readonly UpdateBookingValidator _validator = new();

    [Fact]
    public async Task Validate_WhenAllFieldsAreValid_PassesValidation()
    {
        // Arrange
        var dto = BuildValidDto();

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenTitleIsEmpty_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Title = "";

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Booking title is required");
    }

    [Fact]
    public async Task Validate_WhenTitleExceeds200Characters_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.Title = new string('X', 201);

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WhenOrganizerNameIsEmpty_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.OrganizerName = "";

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Organizer name is required");
    }

    [Fact]
    public async Task Validate_WhenEmailIsInvalidFormat_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.OrganizerEmail = "not-an-email";

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Please provide a valid email");
    }

    [Fact]
    public async Task Validate_WhenEmailIsEmpty_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.OrganizerEmail = "";

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Email is required");
    }

    [Fact]
    public async Task Validate_WhenRoomIdIsZero_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.RoomId = 0;

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Please select a valid room");
    }

    [Fact]
    public async Task Validate_WhenEndTimeIsBeforeStartTime_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.StartTime = DateTime.UtcNow.AddDays(1);
        dto.EndTime = dto.StartTime.AddHours(-1);

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "End time must be after start time");
    }

    private static UpdateBookingDto BuildValidDto() => new()
    {
        Title = "Updated Team Meeting",
        OrganizerName = "Jane Smith",
        OrganizerEmail = "jane@company.com",
        StartTime = DateTime.UtcNow.AddDays(1),
        EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
        RoomId = 1
    };
}
