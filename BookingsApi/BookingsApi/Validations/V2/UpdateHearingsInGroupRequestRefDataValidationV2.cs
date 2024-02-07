using BookingsApi.Contract.V2.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateHearingsInGroupRequestRefDataValidationV2 : RefDataInputValidatorValidator<UpdateHearingsInGroupRequestV2>
    {
        public UpdateHearingsInGroupRequestRefDataValidationV2(List<VideoHearing> hearingsInGroup)
        {
            RuleForEach(x=> x.Hearings).Custom((requestHearing, context) =>
            {
                ValidateHearing(requestHearing, hearingsInGroup, context);
            });
        }

        private static void ValidateHearing(HearingRequestV2 requestHearing, 
            List<VideoHearing> hearingsInGroup, 
            ValidationContext<UpdateHearingsInGroupRequestV2> context)
        {
            var groupId = hearingsInGroup.First().SourceId.Value;
            
            if (!hearingsInGroup.Exists(h => h.Id == requestHearing.HearingId))
            {
                context.AddFailure($"Hearing {requestHearing.HearingId} does not belong to group {groupId}");
            }
        }
    }
}
