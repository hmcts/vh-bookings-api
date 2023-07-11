using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using FluentValidation;
using System;

namespace BookingsApi.Validations
{
    public class CancelBookingRequestValidation : AbstractValidator<CancelBookingRequest>
    {
        public CancelBookingRequestValidation()
        {
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage("UpdatedBy is required");
            RuleFor(x => x.CancelReason).NotEmpty().WithMessage("Cancel reason is required");
        }
    }
}
