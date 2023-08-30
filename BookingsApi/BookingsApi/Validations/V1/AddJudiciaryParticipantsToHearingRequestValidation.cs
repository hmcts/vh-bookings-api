using BookingsApi.Contract.V1.Requests;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BookingsApi.Validations.V1
{
    public abstract class AddJudiciaryParticipantsToHearingRequestValidation : AbstractValidator<List<JudiciaryParticipantRequest>>
    {
        public const string NoParticipantsErrorMessage = "Please provide at least one participant";
        
        public static async Task<ValidationResult> ValidateAsync(List<JudiciaryParticipantRequest> requests)
        {
            var result = new ValidationResult();

            if (requests == null || !requests.Any())
            {
                return new ValidationResult(new List<ValidationFailure> { new("Participants", NoParticipantsErrorMessage) });
            }

            var i = 0;
            
            foreach (var request in requests)
            {
                var validationResult = await new JudiciaryParticipantRequestValidation().ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        result.Errors.Add(new ValidationFailure($"[{i}].{error.PropertyName}", error.ErrorMessage));
                    }
                }

                i++;
            }

            return result;
        }
    }
}
