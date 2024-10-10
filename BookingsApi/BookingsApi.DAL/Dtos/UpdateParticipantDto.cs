using BookingsApi.DAL.Commands;

namespace BookingsApi.DAL.Dtos;

public record UpdateParticipantCommandRequiredDto(Guid HearingId, Guid ParticipantId, string Title, string DisplayName,
    string TelephoneNumber, string OrganisationName, List<LinkedParticipantDto> LinkedParticipants);
    
public record UpdateParticipantCommandOptionalDto(RepresentativeInformation RepresentativeInformation,
    AdditionalInformation AdditionalInformation, string ContactEmail, string InterpreterLanguageCode, string OtherLanguage,
    ScreeningDto Screening, string ExternalReferenceId, string MeasuresExternalId);