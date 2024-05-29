using BookingsApi.Domain.Dtos;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Helper;

public static class ParticipantEndpointHelper
{
    public static IEnumerable<(Participant, LinkedParticipantType)> GetEndpointParticipants(
        List<Participant> hearingParticipants, List<NewEndpointParticipantDto> endpointParticipants)
    {
        if (endpointParticipants == null || !endpointParticipants.Any())
        {
            return new List<(Participant, LinkedParticipantType)>();
        }

        return endpointParticipants.Select(dto
            => (hearingParticipants.Find(x =>
                    x.Person.ContactEmail.Equals(dto.ContactEmail, StringComparison.CurrentCultureIgnoreCase)), dto.Type));
    }

}