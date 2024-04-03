using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class UpdateHearingsInGroupRequestInputValidation : AbstractValidator<UpdateHearingsInGroupRequest>
    {
        public const string NoHearingsErrorMessage = "Please provide at least one hearing";
        public const string DuplicateHearingIdsMessage = "Hearing ids must be unique";
        public const string NoUpdatedByErrorMessage = "UpdatedBy is missing";
        public const string DuplicateScheduledDateTimesMessage = "ScheduledDateTime must be unique for all hearings";

        public UpdateHearingsInGroupRequestInputValidation()
        {
            RuleFor(x => x.Hearings)
                .Must(h => h is { Count: > 0 })
                .WithMessage(NoHearingsErrorMessage);
            
            RuleFor(x => x.Hearings)
                .Must(h => !HasDuplicateHearingIds(h))
                .WithMessage(DuplicateHearingIdsMessage)
                .When(x => x.Hearings != null && x.Hearings.Any());
            
            RuleFor(x => x.Hearings)
                .Must(h => !HasDuplicateScheduledDateTimes(h))
                .WithMessage(DuplicateScheduledDateTimesMessage)
                .When(x => x.Hearings != null && x.Hearings.Any());
            
            RuleForEach(x => x.Hearings)
                .SetValidator(new HearingRequestValidation());
            
            RuleFor(x => x.UpdatedBy)
                .NotEmpty().WithMessage(NoUpdatedByErrorMessage);
        }
        
        private static bool HasDuplicateHearingIds(IEnumerable<HearingRequest> hearings)
        {
            var duplicateHearingIds = hearings
                .GroupBy(x => x.HearingId)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();

            return duplicateHearingIds.Any();
        }
        
        private static bool HasDuplicateScheduledDateTimes(IEnumerable<HearingRequest> hearings)
        {
            var duplicateScheduledDateTimes = hearings
                .GroupBy(x => x.ScheduledDateTime)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();

            return duplicateScheduledDateTimes.Any();
        }
    }
}
