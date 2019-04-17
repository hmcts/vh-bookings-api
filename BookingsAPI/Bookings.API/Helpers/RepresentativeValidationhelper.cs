using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Bookings.API.Helpers
{
    public static class RepresentativeValidationHelper
    {
        public static ValidationResult ValidateRepresentativeInfo(List<string> roles, List<ParticipantRequest> participantRequests)
        {
            ValidationResult validationResult = new ValidationResult();
            foreach (ParticipantRequest participantRequest in participantRequests)
            {
                if (roles.Contains(participantRequest.HearingRoleName))
                {
                    validationResult = new RepresentativeValidation().Validate(participantRequest);
                    if (!validationResult.IsValid)
                    {
                        return validationResult;
                    }
                }
            }
            return validationResult;
        }
    }
}
