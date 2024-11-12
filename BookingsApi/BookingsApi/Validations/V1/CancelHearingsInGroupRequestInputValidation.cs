using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1;

public class CancelHearingsInGroupRequestInputValidation : AbstractValidator<CancelHearingsInGroupRequest>
{
    public const string NoHearingsErrorMessage = "Please provide at least one hearing";
    public const string DuplicateHearingIdsMessage = "Hearing ids must be unique";
        
    public CancelHearingsInGroupRequestInputValidation()
    {
        RuleFor(x => x.HearingIds)
            .Must(h => h is { Count: > 0 })
            .WithMessage(NoHearingsErrorMessage);
            
        RuleFor(x => x.HearingIds)
            .Must(h => !HasDuplicateHearingIds(h))
            .WithMessage(DuplicateHearingIdsMessage)
            .When(x => x.HearingIds != null && x.HearingIds.Count != 0);
    }
        
    private static bool HasDuplicateHearingIds(IEnumerable<Guid> hearingIds)
    {
        var duplicateHearingIds = hearingIds
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();

        return duplicateHearingIds.Any();
    }
}