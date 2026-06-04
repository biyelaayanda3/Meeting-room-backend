using FluentValidation;
using MeetingRoomBooking.Application.DTOs;
namespace MeetingRoomBooking.Application.Validators;

//I added validation rules for CreateBookingDto properties
public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Booking title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.OrganizerName)
            .NotEmpty()
            .WithMessage("Organizer name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.OrganizerEmail)
            .NotEmpty()
            .WithMessage("Organizer email is required")
            .EmailAddress()
            .WithMessage("Please provide a valid email address");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("Please select a valid room");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required")
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Start time must be in the future");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required")
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time");
    }
}