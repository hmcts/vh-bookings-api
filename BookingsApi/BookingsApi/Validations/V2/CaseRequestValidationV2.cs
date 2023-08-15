using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class CaseRequestValidationV2 : AbstractValidator<CaseRequestV2>
    {
        public const string CaseNumberMessage = "Case number is required";
        public const string CaseNameMessage = "Case name is required";

        public CaseRequestValidationV2()
        {
            RuleFor(x => x.Number).NotEmpty().WithMessage(CaseNumberMessage);
            RuleFor(x => x.Name).NotEmpty().WithMessage(CaseNameMessage);
        }
    }
}