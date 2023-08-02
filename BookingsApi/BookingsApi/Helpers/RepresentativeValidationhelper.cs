using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations;
using BookingsApi.Validations.V1;
using FluentValidation.Results;

namespace BookingsApi.Helpers
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
