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
            var updatedByIsRequired = "UpdatedBy is required";
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage(updatedByIsRequired);
        }
    }
}
