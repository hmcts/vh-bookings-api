namespace BookingsApi.Domain.Validations;

public static class DomainRuleErrorMessages
{
    public const string HearingNeedsAHost = "A hearing must have at least one host";
    public const string JudiciaryParticipantNotFound = "Judiciary participant does not exist in the hearing";
    public const string ParticipantWithJudgeRoleAlreadyExists = "A participant with Judge role already exists in the hearing";
    public static string JudiciaryPersonAlreadyExists(string personalCode) => $"A judiciary person {personalCode} already exists in the hearing";
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
    public const string FirstNameRequired = "FirstName cannot be empty";
    public const string LastNameRequired = "LastName cannot be empty";
    public const string CannotAddJudgeWhenJudiciaryJudgeAlreadyExists = "Cannot add judge when judiciary judge already exists";
    public const string CannotAddJudiciaryJudgeWhenJudgeAlreadyExists = "Cannot add judiciary judge when non-judiciary judge already exists";
    public const string HearingNotMultiDay = "Hearing is not multi-day";
    public const string CannotBeOnSameDateAsOtherHearingInGroup = "Hearing cannot be on the same date as another day of the multi-day hearing";
    public const string LanguageAndOtherLanguageCannotBeSet = "Language and OtherLanguage cannot be set at the same time";
    public const string CannotAddLeaverJudiciaryPerson = "Cannot add a participant who is a leaver";
    public const string CannotAddDeletedJudiciaryPerson = "Cannot add a participant who has been deleted";
}