using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class AddParticipantsToHearingRequestInputValidationV2 : AbstractValidator<AddParticipantsToHearingRequestV2>
    {
        public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";

        public AddParticipantsToHearingRequestInputValidationV2()
        {
            RuleFor(x => x.Participants)
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);

            RuleForEach(x => x.Participants)
                .SetValidator(new ParticipantRequestValidationV2());
        }
    }

    public class AddParticipantsToHearingRequestDataValidationV2 : AbstractValidator<AddParticipantsToHearingRequestV2>
    {
        public AddParticipantsToHearingRequestDataValidationV2(CaseType caseType)
        {
            if (caseType == null)
            {
                return;
            }
            
            var representativeRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsRepresentative).Select(x => x.Name).ToList();
            RuleForEach(request => request.Participants).Where(x => representativeRoles.Contains(x.HearingRoleName))
                .SetValidator(new RepresentativeValidation());
        }
    }
}