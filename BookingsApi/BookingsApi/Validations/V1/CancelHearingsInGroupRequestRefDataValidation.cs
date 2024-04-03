using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.Common;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class CancelHearingsInGroupRequestRefDataValidation : RefDataInputValidatorValidator<CancelHearingsInGroupRequest>
    {
        public CancelHearingsInGroupRequestRefDataValidation(List<VideoHearing> hearingsInGroup)
        {
            RuleForEach(x=> x.HearingIds).Custom((requestHearingId, context) =>
            {
                ValidateHearing(requestHearingId, hearingsInGroup, context);
            });
        }

        private static void ValidateHearing(Guid requestHearingId, 
            List<VideoHearing> hearingsInGroup, 
            ValidationContext<CancelHearingsInGroupRequest> context)
        {
            var groupId = hearingsInGroup[0].SourceId.Value;
            
            if (!hearingsInGroup.Exists(h => h.Id == requestHearingId))
            {
                context.AddFailure($"Hearing {requestHearingId} does not belong to group {groupId}");
            }
        }
    }
}
