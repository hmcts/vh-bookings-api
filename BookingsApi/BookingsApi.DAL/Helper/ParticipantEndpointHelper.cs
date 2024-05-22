using BookingsApi.Domain.Dtos;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Helper;

public static class ParticipantEndpointHelper
{
    public static IEnumerable<(Participant, LinkedParticipantType)> GetEndpointParticipants(IEnumerable<Participant> hearingParticipants, IEnumerable<EndpointParticipantDto> endpointParticipants)
    {
        return GetEndpointParticipants(hearingParticipants.ToArray(), endpointParticipants.Select(ep => (ep.ContactEmail, ep.Type)));
    }
    
    public static IEnumerable<(Participant, LinkedParticipantType)> GetEndpointParticipants(Participant[] hearingParticipants, IEnumerable<(string ContactEmail, LinkedParticipantType Type)> endpointParticipants)
    {
        return endpointParticipants.Select(p => (hearingParticipants.First(x => x.Person.ContactEmail.Equals(p.ContactEmail, StringComparison.CurrentCultureIgnoreCase)), p.Type));
    }
}