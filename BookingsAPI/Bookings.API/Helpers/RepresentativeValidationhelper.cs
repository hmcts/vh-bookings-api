using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Bookings.API.Helpers
{
    public static class RepresentativeValidationHelper
    {
        public static ValidationResult ValidateRepresentativeInfo(List<ParticipantRequest> participantRequests)
        {
            ValidationResult validationResult = new ValidationResult();
            foreach (ParticipantRequest participantRequest in participantRequests)
            {
                validationResult = new RepresentativeValidation().Validate(participantRequest);
                if (!validationResult.IsValid)
                {
                    return validationResult;
                }

            }
            return validationResult;
        }
    }
}
