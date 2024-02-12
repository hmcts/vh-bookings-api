using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class UpdateHearingParticipantsRequestRefDataValidation : RefDataInputValidatorValidator<UpdateHearingParticipantsRequest>
    {
        public UpdateHearingParticipantsRequestRefDataValidation(CaseType caseType)
        {
            var representativeRoles = caseType.CaseRoles
                .SelectMany(x => x.HearingRoles)
                .Where(x => x.UserRole.IsRepresentative)
                .Select(x => x.Name)
                .ToList();
            
            RuleForEach(request => request.NewParticipants).Where(x => representativeRoles.Contains(x.HearingRoleName))
                .SetValidator(new RepresentativeValidation());
        }
    }
}
