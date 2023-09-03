using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class UpdateHearingParticipantsRequestDataValidationV2 : AbstractValidator<UpdateHearingParticipantsRequestV2>
{
    public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";

    public UpdateHearingParticipantsRequestDataValidationV2(CaseType caseType, List<HearingRole> hearingRoles)
    {
        RuleForEach(x=> x.NewParticipants).Custom((participant, context) =>
        {
            ValidateHearingRole(participant, caseType, hearingRoles, context);
        });
            
        var representativeRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsRepresentative).Select(x => x.Name).ToList();
            
        RuleForEach(request => request.NewParticipants).Where(x => representativeRoles.Contains(x.HearingRoleName))
            .SetValidator(new RepresentativeValidation());
    }
    
    private void ValidateHearingRole(ParticipantRequestV2 participant, CaseType caseType, List<HearingRole> hearingRoles, ValidationContext<UpdateHearingParticipantsRequestV2> context)
    {
        // if no case role is provided, this request is using the flat structure
        if (string.IsNullOrEmpty(participant.CaseRoleName))
        {
            if (!hearingRoles.Exists(x => x.Name == participant.HearingRoleName))
            {
                context.AddFailure($"Invalid hearing role [{participant.HearingRoleName}]");
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
        if (caseType == null)
        {
            return;
        }

        var caseRole = caseType.CaseRoles.Find(x => x.Name == participant.CaseRoleName);
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