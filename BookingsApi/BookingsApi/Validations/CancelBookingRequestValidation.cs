using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using FluentValidation;
using System;

namespace BookingsApi.Validations
{
    public class CancelBookingRequestValidation : AbstractValidator<CancelBookingRequest>
    {
        public const string UpdatedByMessage = "UpdatedBy is required";
        public const string CancelReasonMessage = "Cancel reason is required";

        public CancelBookingRequestValidation()
        {
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage(UpdatedByMessage);
            RuleFor(x => x.CancelReason).NotEmpty().WithMessage(CancelReasonMessage);
        }
    }
}
