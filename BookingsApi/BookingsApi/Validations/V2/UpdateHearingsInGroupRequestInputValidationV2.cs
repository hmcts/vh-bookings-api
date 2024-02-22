using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateHearingsInGroupRequestInputValidationV2 : AbstractValidator<UpdateHearingsInGroupRequestV2>
    {
        public const string NoHearingsErrorMessage = "Please provide at least one hearing";
        public const string DuplicateHearingIdsMessage = "Hearing ids must be unique";
        public const string NoUpdatedByErrorMessage = "UpdatedBy is missing";

        public UpdateHearingsInGroupRequestInputValidationV2()
        {
            RuleFor(x => x.Hearings)
                .Must(h => h is { Count: > 0 })
                .WithMessage(NoHearingsErrorMessage);
            
            RuleFor(x => x.Hearings)
                .Must(h => !HasDuplicateHearingIds(h))
                .WithMessage(DuplicateHearingIdsMessage)
                .When(x => x.Hearings != null && x.Hearings.Any());

            RuleForEach(x => x.Hearings)
                .SetValidator(new HearingRequestInputValidationV2());
            
            RuleFor(x => x.UpdatedBy)
                .NotEmpty().WithMessage(NoUpdatedByErrorMessage);
        }
        
        private static bool HasDuplicateHearingIds(IEnumerable<HearingRequestV2> hearings)
        {
            var duplicateHearingIds = hearings
                .GroupBy(x => x.HearingId)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();

            return duplicateHearingIds.Any();
        }
    }
}
