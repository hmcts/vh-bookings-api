using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using FluentValidation;
using System;

namespace BookingsApi.Validations
{
    public class UpdateBookingStatusRequestValidation : AbstractValidator<UpdateBookingStatusRequest>
    {
        public UpdateBookingStatusRequestValidation()
        {
            var updatedByIsRequired = "UpdatedBy is required";
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage(updatedByIsRequired);

            RuleFor(x => x.Status).Custom((r, context) =>
            {
                if (!Enum.IsDefined(typeof(UpdateBookingStatus), r))
                    context.AddFailure("The booking status is not recognised");
            });

            var cancelReasonIsRequired = "Cancel reason is required when a hearing is cancelled";
            RuleFor(x => x.CancelReason)
                .NotEmpty()
                .When(x => x.Status == UpdateBookingStatus.Cancelled)
                .WithMessage(cancelReasonIsRequired);
        }
    }
}
