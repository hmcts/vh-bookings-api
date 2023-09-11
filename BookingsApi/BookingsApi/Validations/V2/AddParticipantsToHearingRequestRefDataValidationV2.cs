using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class AddParticipantsToHearingRequestRefDataValidationV2 : RefDataInputValidatorValidator<AddParticipantsToHearingRequestV2>
{
    public AddParticipantsToHearingRequestRefDataValidationV2(CaseType caseType)
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