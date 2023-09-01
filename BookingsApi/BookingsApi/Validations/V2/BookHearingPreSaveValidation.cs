using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2;

public class BookHearingPreSaveValidation : AbstractValidator<BookNewHearingRequestV2>
{
    public BookHearingPreSaveValidation(CaseType caseType, HearingVenue hearingVenue)
    {
        RuleFor(x => x.ServiceId).Custom((serviceId, context) =>
        {
            if (caseType == null)
            {
                context.AddFailure("Case type does not exist");
            }
        });
            
        RuleFor(x=>x.HearingTypeCode).Custom((hearingTypeCode, context) =>
        {
            if (caseType?.HearingTypes == null || caseType.HearingTypes.TrueForAll(x =>
                    !x.Code.Equals(hearingTypeCode, StringComparison.CurrentCultureIgnoreCase)))
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
    }
}