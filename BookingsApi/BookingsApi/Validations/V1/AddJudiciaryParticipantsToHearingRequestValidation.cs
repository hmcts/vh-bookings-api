using BookingsApi.Contract.V1.Requests;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BookingsApi.Validations.V1
{
    public class AddJudiciaryParticipantsToHearingRequestValidation : AbstractValidator<List<JudiciaryParticipantRequest>>
    {
        public const string NoParticipantsErrorMessage = "Please provide at least one participant";

        public AddJudiciaryParticipantsToHearingRequestValidation()
        {
            RuleFor(x => x).NotEmpty().WithMessage(NoParticipantsErrorMessage);
        }

        public async Task<ValidationResult> ValidateRequestsAsync(List<JudiciaryParticipantRequest> requests)
        {
            var result = await ValidateAsync(requests);
            
            if (!result.IsValid)
            {
                return result;
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
