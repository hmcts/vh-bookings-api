using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class UpdateHearingParticipantsRequestRefDataValidationV2 : RefDataInputValidatorValidator<UpdateHearingParticipantsRequestV2>
{
    public UpdateHearingParticipantsRequestRefDataValidationV2(List<HearingRole> hearingRoles)
    {
        RuleForEach(x=> x.NewParticipants).Custom((participant, context) =>
        {
            ValidateHearingRole(participant, hearingRoles, context);
        });
    }

    private static void ValidateHearingRole(ParticipantRequestV2 participant, List<HearingRole> hearingRoles, ValidationContext<UpdateHearingParticipantsRequestV2> context)
    {
        if (!hearingRoles.Exists(x => string.Compare(x.Code, participant.HearingRoleCode, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            context.AddFailure($"Invalid hearing role [{participant.HearingRoleCode}]");
        }
    }
}