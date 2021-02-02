using BookingsApi.Contract.Requests;
using System.Collections.Generic;
using BookingsApi.Validations;
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
