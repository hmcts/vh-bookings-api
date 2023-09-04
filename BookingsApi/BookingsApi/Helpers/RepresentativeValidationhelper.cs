using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.Common;
using FluentValidation.Results;

namespace BookingsApi.Helpers
{
    public static class RepresentativeValidationHelper
    {
        /// <summary>
        /// Method to validate the representative info for V1
        /// </summary>
        /// <param name="participantRequests"></param>
        /// <returns></returns>
        public static ValidationResult ValidateRepresentativeInfo(List<ParticipantRequest> participantRequests)
        {
            ValidationResult validationResult = new ValidationResult();
            foreach (var participantRequest in participantRequests)
            {
                validationResult = new RepresentativeValidation().Validate(participantRequest);
                if (!validationResult.IsValid)
                    return validationResult;
            }
            return validationResult;
        }
    }
}
