using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class UpdateHearingParticipantsRequestRefDataValidationV2 : RefDataInputValidatorValidator<UpdateHearingParticipantsRequestV2>
{
    public UpdateHearingParticipantsRequestRefDataValidationV2(CaseType caseType, List<HearingRole> hearingRoles)
    {
        RuleForEach(x=> x.NewParticipants).Custom((participant, context) =>
        {
            ValidateHearingRole(participant, caseType, hearingRoles, context);
        });
            
        var representativeRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsRepresentative).Select(x => x.Name).ToList();
            
        RuleForEach(request => request.NewParticipants).Where(x => representativeRoles.Contains(x.HearingRoleName))
            .SetValidator(new RepresentativeValidation());
    }
    
    private static void ValidateHearingRole(ParticipantRequestV2 participant, CaseType caseType, List<HearingRole> hearingRoles, ValidationContext<UpdateHearingParticipantsRequestV2> context)
    {
        // if no case role is provided, this request is using the flat structure
        if (string.IsNullOrEmpty(participant.CaseRoleName))
        {
            if (!hearingRoles.Exists(x => x.Code == participant.HearingRoleCode))
            {
                context.AddFailure($"Invalid hearing role [{participant.HearingRoleCode}]");
            }
        }
        else
        {
            ValidateRoleFromCaseType(participant, caseType, context);
        }
    }
    
    private static void ValidateRoleFromCaseType(ParticipantRequestV2 participant, CaseType caseType,
        ValidationContext<UpdateHearingParticipantsRequestV2> context)
    {
        var caseRole = caseType?.CaseRoles.Find(x => x.Name == participant.CaseRoleName);
        if (caseRole == null)
        {
            context.AddFailure($"Invalid case role [{participant.CaseRoleName}]");
            return;
        }

        if (!caseRole.HearingRoles.Exists(x => x.Name == participant.HearingRoleName))
        {
            context.AddFailure($"Invalid hearing role [{participant.HearingRoleName}]");
        }
    }
}