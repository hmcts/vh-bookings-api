using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class EndpointDtoMapper
    {
            
        public static EndpointDto MapToDto(Endpoint source, IList<Participant> participants, IList<Endpoint> endpoints)
        {
            return new EndpointDto
            {
                DisplayName = source.DisplayName,
                Sip = source.Sip,
                Pin = source.Pin,
                ParticipantsLinked = source.ParticipantsLinked.Select(x => x.Person.ContactEmail).ToList(),
                Role = source.GetEndpointConferenceRole(participants, endpoints)
            };
        }
    
    
        /// <summary>
        /// Determine the role of the endpoint in the conference. If an endpoint is screened from any participant or endpoint, it is a 'Guest'.
        /// Otherwise, they are a 'Host'.
        /// </summary>
        /// <param name="source">Endpoint to check for screening requirements</param>
        /// <param name="participants">List of participants to check if endpoint is being screened from</param>
        /// <param name="endpoints">List of endpoints to check if endpoint is being screened from</param>
        /// <returns>'Host' or 'Guest'</returns>
        public static ConferenceRole GetEndpointConferenceRole(this Endpoint source, IList<Participant> participants, IList<Endpoint> endpoints)
        {
            var noScreeningRequired =
                participants.All(x => x.Screening == null || x.Screening.ScreeningEntities.Count == 0) &&
                endpoints.All(x => x.Screening == null || x.Screening.ScreeningEntities.Count == 0);
            
            // if no endpoint or participant requires screening, then the default role is 'Guest'
            if (noScreeningRequired) return ConferenceRole.Guest;
            
            var endpointRequiresScreening = source.Screening != null && (source.Screening.Type == ScreeningType.All ||
                                                                 source.Screening.ScreeningEntities.Count > 0);
            if (endpointRequiresScreening) return ConferenceRole.Guest;

            var participantScreenings = participants.Where(x => x.Screening != null).SelectMany(x => x.Screening.GetEndpoints()).ToList();
            var endpointScreenings = endpoints.Where(x => x.Screening != null).SelectMany(x => x.Screening.GetEndpoints()).ToList();

            var screenedFrom = participantScreenings.Exists(se => se.EndpointId == source.Id) ||
                               endpointScreenings.Exists(se => se.EndpointId == source.Id);

            return screenedFrom ? ConferenceRole.Guest : ConferenceRole.Host;
        }
    }
}