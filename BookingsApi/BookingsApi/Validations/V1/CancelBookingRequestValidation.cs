﻿using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
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
