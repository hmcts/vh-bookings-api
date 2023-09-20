namespace BookingsApi.Domain.Validations;

public static class DomainRuleErrorMessages
{
    public const string HearingNeedsAHost = "A hearing must have at least one host";
    public const string JudiciaryParticipantNotFound = "Judiciary participant does not exist in the hearing";
    public const string ParticipantWithJudgeRoleAlreadyExists = "A participant with Judge role already exists in the hearing";
    public const string CannotEditACancelledHearing = "Cannot edit a cancelled hearing";
    public const string CannotEditAHearingCloseToStartTime = "Cannot edit a hearing close to the scheduled start time";
    public const string CannotAddInterpreterToHearingCloseToStartTime = "Cannot add an interpreter to a hearing close to the scheduled start time";
}