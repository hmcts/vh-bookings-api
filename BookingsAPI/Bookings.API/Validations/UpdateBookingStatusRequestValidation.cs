using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Requests.Enums;
using FluentValidation;
using System;

namespace Bookings.API.Validations
{
    public class UpdateBookingStatusRequestValidation : AbstractValidator<UpdateBookingStatusRequest>
    {
        public UpdateBookingStatusRequestValidation()
        {
            var updatedByIsRequired = "UpdatedBy is required";
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage(updatedByIsRequired);

            var bookingStatusIsNotRecognised = "The booking status is not recognised";
            RuleFor(x => x.Status).Custom((r, context) =>
            {
                if (!Enum.IsDefined(typeof(UpdateBookingStatus), r))
                    context.AddFailure(bookingStatusIsNotRecognised);
            });

            var cancelReasonIsRequired = "Cancel reason is required when a hearing is cancelled";
            RuleFor(x => x.CancelReason)
                .NotEmpty()
                .When(x => x.Status == UpdateBookingStatus.Cancelled)
                .WithMessage(cancelReasonIsRequired);
        }
    }
}
