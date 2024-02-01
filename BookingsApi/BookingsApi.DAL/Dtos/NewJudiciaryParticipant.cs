namespace BookingsApi.DAL.Dtos;

public class NewJudiciaryParticipant
{
    public string DisplayName { get; set; }
    public string PersonalCode { get; set; }
    public string OptionalContactEmail { get; set; }
    public string OptionalContactTelephone { get; set; }
    public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; set; }
}