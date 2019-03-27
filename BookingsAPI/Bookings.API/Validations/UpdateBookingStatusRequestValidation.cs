using Bookings.Api.Contract.Requests;
using Bookings.Domain.Enumerations;
using FluentValidation;
using System;

namespace Bookings.API.Validations
{
    public class UpdateBookingStatusRequestValidation : AbstractValidator<UpdateBookingStatusRequest>
    {
        public UpdateBookingStatusRequestValidation()
        {
            var bookingStatusIsRequired = "Booking Status is required";
            var bookingStatusIsNotRecognised = "The booking status is not recognised";
            var updatedByIsRequired = "UpdatedBy is required";
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage(updatedByIsRequired);
            RuleFor(x => x.Status).NotEmpty().WithMessage(bookingStatusIsRequired);
            RuleFor(x => x.Status).Custom((r, context) =>
            {
                if (!string.IsNullOrEmpty(r) && !Enum.IsDefined(typeof(BookingStatus), r))
                    context.AddFailure(bookingStatusIsNotRecognised);
            });
        }
    }
}
