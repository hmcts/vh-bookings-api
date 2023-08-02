using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
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