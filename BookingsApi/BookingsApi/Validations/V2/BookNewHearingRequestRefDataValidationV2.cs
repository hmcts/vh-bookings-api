using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class BookNewHearingRequestRefDataValidationV2 : RefDataInputValidatorValidator<BookNewHearingRequestV2>
{
    public BookNewHearingRequestRefDataValidationV2(CaseType caseType, HearingVenue hearingVenue, List<HearingRole> hearingRoles)
    {
        RuleFor(x => x.ServiceId).Custom((_, context) =>
        {
            if (caseType == null)
            {
                context.AddFailure("Case type does not exist");
            }
        });
            
        RuleFor(x=>x.HearingTypeCode).Custom((hearingTypeCode, context) =>
        {
            if (hearingTypeCode != null && 
                (caseType?.HearingTypes == null || caseType.HearingTypes.TrueForAll(x =>
                    !x.Code.Equals(hearingTypeCode, StringComparison.CurrentCultureIgnoreCase))))
            {
                context.AddFailure($"Hearing type code {hearingTypeCode} does not exist");
            }
        });
            
        RuleFor(x=>x.HearingVenueCode).Custom((hearingVenueCode, context) =>
        {
            if (hearingVenue == null)
            {
                context.AddFailure($"HearingVenueCode {hearingVenueCode} does not exist");
            }
        });
        
        RuleForEach(x=> x.Participants).Custom((participant, context) =>
        {
            ValidateHearingRole(participant, caseType, hearingRoles, context);
        });
        
    }

    private static void ValidateHearingRole(ParticipantRequestV2 participant, CaseType caseType, List<HearingRole> hearingRoles, ValidationContext<BookNewHearingRequestV2> context)
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
        ValidationContext<BookNewHearingRequestV2> context)
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