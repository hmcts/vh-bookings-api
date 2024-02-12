using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class UpdateHearingsInGroupRequestRefDataValidation : RefDataInputValidatorValidator<UpdateHearingsInGroupRequest>
    {
        public UpdateHearingsInGroupRequestRefDataValidation(List<VideoHearing> hearingsInGroup)
        {
            RuleForEach(x=> x.Hearings).Custom((requestHearing, context) =>
            {
                ValidateHearing(requestHearing, hearingsInGroup, context);
            });
        }
        
        private static void ValidateHearing(HearingRequest requestHearing, 
            List<VideoHearing> hearingsInGroup, 
            ValidationContext<UpdateHearingsInGroupRequest> context)
        {
            var groupId = hearingsInGroup[0].SourceId.Value;
            
            if (!hearingsInGroup.Exists(h => h.Id == requestHearing.HearingId))
            {
                context.AddFailure($"Hearing {requestHearing.HearingId} does not belong to group {groupId}");
            }
        }
    }
}
