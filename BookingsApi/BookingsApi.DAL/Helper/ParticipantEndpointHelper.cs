using BookingsApi.Domain.Dtos;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Helper;

public static class ParticipantEndpointHelper
{
    public static IEnumerable<(Participant, LinkedParticipantType)> GetEndpointParticipants(List<Participant> hearingParticipants, List<NewEndpointParticipantDto> endpointParticipants)
    {
        return endpointParticipants.Select(dto 
            => (hearingParticipants.First(x => x.Person.ContactEmail.Equals(dto.ContactEmail, StringComparison.CurrentCultureIgnoreCase)), dto.Type));
    }
    
}