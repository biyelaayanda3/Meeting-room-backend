using FluentValidation;
using MeetingRoomBooking.Application.DTOs;
namespace MeetingRoomBooking.Application.Validators;

public class UpdateBookingValidator : AbstractValidator<UpdateBookingDto>
{
    public UpdateBookingValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Booking title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.OrganizerName)
            .NotEmpty()
            .WithMessage("Organizer name is required");

        RuleFor(x => x.OrganizerEmail)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Please provide a valid email");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("Please select a valid room");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time");
    }
}