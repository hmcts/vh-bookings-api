using System.Collections.Generic;
using Bookings.Domain;
using Bookings.Domain.Participants;

namespace Bookings.API.DomainServices
{
    public interface IHearingService
    {
        Hearing AddParticipantsToHearing(Hearing hearing, IEnumerable<Participant> participants);
    }
}