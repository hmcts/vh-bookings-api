using BookingsApi.DAL.Commands;

namespace BookingsApi.DAL.Dtos;

public class ExistingParticipantDetails
{
    public Guid ParticipantId { get; set; }
    public string Title { get; set; }
    public string DisplayName { get; set; }
    public string TelephoneNumber { get; set; }
    public string CaseRoleName { get; set; }
    public string HearingRoleName { get; set; }
    public string OrganisationName { get; set; }
    public Person Person { get; set; }
    public RepresentativeInformation RepresentativeInformation { get; set; }
    public bool IsContactEmailNew { get; set; }
    public string InterpreterLanguageCode { get; set; }
    public string OtherLanguage { get; set; }
    public ScreeningDto Screening { get; set; }
    public string ExternalReferenceId { get; set; }
    public string MeasuresExternalId { get; set; }
}