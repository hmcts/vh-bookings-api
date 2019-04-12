using Bookings.Api.Contract.Requests;
using Bookings.API.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Bookings.API.Helpers
{
    public static class AddressValidationHelper

    {
        public static ValidationResult ValidateAddress(List<string> roles, List<ParticipantRequest> participantRequests)
        {
            ValidationResult validationResult = new ValidationResult();
            foreach (ParticipantRequest participantRequest in participantRequests)
            {
                if (roles.Contains(participantRequest.HearingRoleName))
                {
                    validationResult = new AddressValidation().Validate(participantRequest);
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
