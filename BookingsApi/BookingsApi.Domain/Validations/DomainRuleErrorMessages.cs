namespace BookingsApi.Domain.Validations;

public static class DomainRuleErrorMessages
{
    public const string HearingNeedsAHost = "A hearing must have at least one host";
    public const string JudiciaryParticipantNotFound = "Judiciary participant does not exist in the hearing";
    public const string ParticipantWithJudgeRoleAlreadyExists = "A participant with Judge role already exists in the hearing";
    public const string CannotEditACancelledHearing = "Cannot edit a cancelled hearing";
    public const string DefaultCannotEditAHearingCloseToStartTime = "Cannot edit a hearing close to the scheduled start time";
    public const string CannotRemoveParticipantCloseToStartTime =
        "Cannot remove a participant from hearing that is close to start time";
    public const string CannotRemoveJudiciaryParticipantCloseToStartTime =
        "Cannot remove a judiciary participant from hearing that is close to start time";
    public const string CannotUpdateJudiciaryParticipantCloseToStartTime =
        "Cannot update a judiciary participant from hearing that is close to start time";
    public const string CannotRemoveAnEndpointCloseToStartTime =
        "Cannot remove an endpoint from hearing that is close to start time";
    public const string CannotUpdateACaseCloseToStartTime =
        "Cannot update a case that is close to start time";
    public const string CannotUpdateHearingDetailsCloseToStartTime =
        "Cannot update hearing details that is close to start time";
    public const string CannotAddInterpreterToHearingCloseToStartTime = "Cannot add an interpreter to a hearing close to the scheduled start time";
    public const string FirstNameRequired = "FirstName cannot be empty";
    public const string LastNameRequired = "LastName cannot be empty";
}