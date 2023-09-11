using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class LinkedParticipantRequestValidationV2 : AbstractValidator<LinkedParticipantRequestV2>
    {
        public static readonly string NoParticipantEmail = "Contact email for participant is required";
        public static readonly string NoLinkedParticipantEmail = "Contact email for linked participant is required";
        public static readonly string InvalidType = "A valid linked participant type is required";
        
        public LinkedParticipantRequestValidationV2()
        {
            RuleFor(x => x.ParticipantContactEmail).NotEmpty().WithMessage(NoParticipantEmail);
            RuleFor(x => x.LinkedParticipantContactEmail).NotEmpty().WithMessage(NoLinkedParticipantEmail);
            RuleFor(x => x.Type).NotEmpty().IsInEnum().WithMessage(InvalidType);
        }
    }
}