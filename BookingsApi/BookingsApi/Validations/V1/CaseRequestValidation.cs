using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class CaseRequestValidation : AbstractValidator<CaseRequest>
    {
        public const string CaseNumberMessage = "Case number is required";
        public const string CaseNameMessage = "Case name is required";

        public CaseRequestValidation()
        {
            RuleFor(x => x.Number).NotEmpty().WithMessage(CaseNumberMessage);
            RuleFor(x => x.Name).NotEmpty().WithMessage(CaseNameMessage);
        }
    }
}