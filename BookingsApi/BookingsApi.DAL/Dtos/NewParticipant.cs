namespace BookingsApi.DAL.Dtos;

public class NewParticipant
{
    public NewParticipant()
    {
        
    }
    public Person Person { get; set; }
    [Obsolete("CaseRole is no longer required. Will be removed in a future release")]
    public CaseRole CaseRole { get; set; }
    public HearingRole HearingRole { get; set; }
    public string Representee { get; set; }
    public string DisplayName { get; set; }
    public string InterpreterLanguageCode { get; set; }
    public string OtherLanguage { get; set; }
    public ScreeningDto Screening { get; set; }
    public string ExternalReferenceId { get; set; }
    public string MeasuresExternalId { get; set; }
}