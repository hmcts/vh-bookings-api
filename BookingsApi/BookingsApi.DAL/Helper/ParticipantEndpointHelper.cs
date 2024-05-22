using BookingsApi.Domain.Dtos;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Helper;

public static class ParticipantEndpointHelper
{
    public static IEnumerable<(Participant, LinkedParticipantType)> GetEndpointParticipants(this IEnumerable<Participant> hearingParticipants, IEnumerable<EndpointParticipantDto> endpointParticipants)
    {
        return hearingParticipants.GetEndpointParticipants(endpointParticipants.Select(ep => (ep.ContactEmail, ep.Type)));
    }
    
    public static IEnumerable<(Participant, LinkedParticipantType)> GetEndpointParticipants(this IEnumerable<Participant> hearingParticipants, IEnumerable<(string ContactEmail, LinkedParticipantType Type)> endpointParticipants)
    {
        if (endpointParticipants == null) yield break;
        var participants = hearingParticipants.ToList();
        foreach (var p in endpointParticipants)
            yield return (participants.First(x => x.Person.ContactEmail.Equals(p.ContactEmail, StringComparison.CurrentCultureIgnoreCase)), p.Type);
    }
}