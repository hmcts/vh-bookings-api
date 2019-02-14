using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.Participants;

namespace Bookings.API.DomainServices
{
    /// <summary>
    /// Responsible for safely adding participants to a hearing, either by creating them or referencing existing participants
    /// </summary>
    /// <remarks>
    /// Due to the structure of feeds holding participants, EF now allows us to add new participants
    /// to the system without going through the domain and thereby bypassing the consistency checks.
    /// This service helps us avoid this but if by any chance, a user adds a feed to a hearing without
    /// using this service, they will sidestep the check for existing participant and possibly end up
    /// creating participants with duplicate username.
    /// </remarks>
    public class HearingService : IHearingService
    {
        private readonly IQueryHandler _queryHandler;

        public HearingService(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        public Hearing AddParticipantsToHearing(Hearing hearing, IEnumerable<Participant> participants)
        {
            foreach (var participant in participants)
            {
//                var existingPersonWithUsername = _repository
//                    .GetAllWhere<Person, Guid>(x => x.Username.Equals(participant.Person.Username,
//                        StringComparison.CurrentCultureIgnoreCase)).ToList();
//
//                var existingPerson = existingPersonWithUsername.SingleOrDefault();
//                var participantToAdd =
//                    new Individual(existingPerson ?? participant.Person, participant.HearingRole, participant.CaseRole)
//                    {
//                        DisplayName = participant.DisplayName
//                    };
//                hearing.AddParticipant(participantToAdd);
            }
            

            return hearing;
        }
    }
}