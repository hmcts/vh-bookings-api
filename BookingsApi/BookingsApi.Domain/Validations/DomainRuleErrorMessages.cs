namespace BookingsApi.Domain.Validations;

public static class DomainRuleErrorMessages
{
    public const string HearingNeedsAHost = "A hearing must have at least one host";
    public const string JudiciaryParticipantNotFound = "Judiciary participant does not exist in the hearing";
}