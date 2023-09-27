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
            
        var representativeRoles = hearingRoles.Where(x => x.UserRole.IsRepresentative).Select(x => x.Name).ToList();
            
        RuleForEach(request => request.NewParticipants).Where(x => representativeRoles.Contains(x.HearingRoleCode))
            .SetValidator(new RepresentativeValidation());
    }
    
    private static void ValidateHearingRole(ParticipantRequestV2 participant, List<HearingRole> hearingRoles, ValidationContext<UpdateHearingParticipantsRequestV2> context)
    {
        if (!hearingRoles.Exists(x => x.Code == participant.HearingRoleCode))
        {
            context.AddFailure($"Invalid hearing role [{participant.HearingRoleCode}]");
        }
    }
}