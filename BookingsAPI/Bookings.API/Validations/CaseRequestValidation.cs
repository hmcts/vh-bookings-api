using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
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