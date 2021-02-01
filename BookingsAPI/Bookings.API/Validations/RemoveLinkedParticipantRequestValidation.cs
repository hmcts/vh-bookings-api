using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class RemoveLinkedParticipantRequestValidation : AbstractValidator<RemoveLinkedParticipantRequest>
    {
        public static readonly string NoId = "A link Id is required";
        
        public RemoveLinkedParticipantRequestValidation()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(NoId);
        }
    }
}