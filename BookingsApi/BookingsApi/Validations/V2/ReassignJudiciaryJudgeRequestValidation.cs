using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class ReassignJudiciaryJudgeRequestValidation : AbstractValidator<ReassignJudiciaryJudgeRequest>
    {
        public const string NoPersonalCodeErrorMessage = "Personal code is required";
        public const string NoDisplayNameErrorMessage = "Display name is required";

        public ReassignJudiciaryJudgeRequestValidation()
        {
            RuleFor(x => x.PersonalCode).NotEmpty().WithMessage(NoPersonalCodeErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }
    }
}
