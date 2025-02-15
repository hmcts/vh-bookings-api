namespace BookingsApi.DAL.Dtos;

public class NewParticipant
{
    public Person Person { get; set; }
    public HearingRole HearingRole { get; set; }
    public string Representee { get; set; }
    public string DisplayName { get; set; }
    public string InterpreterLanguageCode { get; set; }
    public string OtherLanguage { get; set; }
    public ScreeningDto Screening { get; set; }
    public string ExternalReferenceId { get; set; }
    public string MeasuresExternalId { get; set; }
}