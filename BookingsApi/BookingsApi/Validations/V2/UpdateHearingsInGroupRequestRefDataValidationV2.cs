using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateHearingsInGroupRequestRefDataValidationV2 : RefDataInputValidatorValidator<UpdateHearingsInGroupRequestV2>
    {
        public UpdateHearingsInGroupRequestRefDataValidationV2(List<VideoHearing> hearingsInGroup, List<HearingRole> hearingRoles, List<HearingVenue> venues)
        {
            RuleForEach(x=> x.Hearings).Custom((requestHearing, context) =>
            {
                ValidateHearing(requestHearing, hearingsInGroup, venues, context);
            });

            RuleForEach(x => x.Hearings)
                .SetValidator(new HearingRequestRefDataValidationV2(hearingRoles));
        }

        private static void ValidateHearing(HearingRequestV2 requestHearing, 
            List<VideoHearing> hearingsInGroup,
            List<HearingVenue> venues,
            ValidationContext<UpdateHearingsInGroupRequestV2> context)
        {
            var groupId = hearingsInGroup[0].SourceId.Value;
            
            if (!hearingsInGroup.Exists(h => h.Id == requestHearing.HearingId))
            {
                context.AddFailure($"Hearing {requestHearing.HearingId} does not belong to group {groupId}");
            }

            if (!venues.Exists(v => v.VenueCode == requestHearing.HearingVenueCode))
            {
                context.AddFailure($"Hearing venue code {requestHearing.HearingVenueCode} does not exist");
            }
        }
    }
}
