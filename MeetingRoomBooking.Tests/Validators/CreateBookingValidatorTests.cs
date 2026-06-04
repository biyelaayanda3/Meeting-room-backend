using FluentAssertions;
using MeetingRoomBooking.Application.DTOs;
using MeetingRoomBooking.Application.Validators;
using Xunit;

namespace MeetingRoomBooking.Tests.Validators;

public class CreateBookingValidatorTests
{
    private readonly CreateBookingValidator _validator = new();

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
    public async Task Validate_WhenTitleIsEmpty_FailsWithTitleRequiredMessage()
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
        dto.Title = new string('A', 201);

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Title cannot exceed 200 characters");
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
    public async Task Validate_WhenOrganizerNameExceeds100Characters_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.OrganizerName = new string('B', 101);

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name cannot exceed 100 characters");
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
        result.Errors.Should().Contain(e => e.ErrorMessage == "Organizer email is required");
    }

    [Fact]
    public async Task Validate_WhenEmailFormatIsInvalid_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.OrganizerEmail = "not-an-email";

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Please provide a valid email address");
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
    public async Task Validate_WhenRoomIdIsNegative_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.RoomId = -1;

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WhenStartTimeIsInThePast_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        dto.StartTime = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Start time must be in the future");
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

    [Fact]
    public async Task Validate_WhenEndTimeEqualsStartTime_FailsValidation()
    {
        // Arrange
        var dto = BuildValidDto();
        var time = DateTime.UtcNow.AddDays(1);
        dto.StartTime = time;
        dto.EndTime = time;

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    private static CreateBookingDto BuildValidDto() => new()
    {
        Title = "Team Meeting",
        OrganizerName = "Jane Smith",
        OrganizerEmail = "jane@company.com",
        StartTime = DateTime.UtcNow.AddDays(1),
        EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
        RoomId = 1
    };
}
