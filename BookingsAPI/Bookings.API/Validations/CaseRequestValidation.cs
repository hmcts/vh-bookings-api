using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class CaseRequestValidation : AbstractValidator<CaseRequest>
    {
        public static readonly string NoCaseNumberMessage = "Case number is required";
        public static readonly string NoCaseNameMessage = "Case name is required";
        
        public CaseRequestValidation()
        {
            RuleFor(x => x.Number).NotEmpty().WithMessage(NoCaseNumberMessage);
            RuleFor(x => x.Name).NotEmpty().WithMessage(NoCaseNameMessage);
        }
    }
}