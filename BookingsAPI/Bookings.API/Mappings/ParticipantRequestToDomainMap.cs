using System.Linq;
using Bookings.Api.Contract.Requests;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;

namespace Bookings.API.Mappings
{
    public class ParticipantRequestToDomainMap
    {
        public Participant MapRequestToDomain(ParticipantRequest requestParticipant, CaseType caseType)
        {
            var person = new Person(requestParticipant.Title, requestParticipant.FirstName, requestParticipant.LastName,
                requestParticipant.Username)
            {
                MiddleNames = requestParticipant.MiddleNames,
                ContactEmail = requestParticipant.ContactEmail,
                TelephoneNumber = requestParticipant.TelephoneNumber
            };

            var caseRole = caseType.CaseRoles.SingleOrDefault(x => x.Name == requestParticipant.CaseRoleName);
            var hearingRole = caseRole?.HearingRoles.SingleOrDefault(x => x.Name == requestParticipant.HearingRoleName);

            Participant participant;
            switch (hearingRole?.UserRole.Name)
            {
                case "Individual":
                    var individual = new Individual(person, hearingRole, caseRole);
                    participant = individual;
                    break;
                case "Representative":
                {
                    var rep = new Representative(person, hearingRole, caseRole)
                    {
                        SolicitorsReference = requestParticipant.SolicitorsReference,
                        Representee = requestParticipant.Representee
                    };
                    participant = rep;
                    break;
                }
                default:
                    throw new DomainRuleException(nameof(hearingRole.UserRole.Name),
                        $"Role {hearingRole?.UserRole.Name} not recognised");
            }

            participant.DisplayName = requestParticipant.DisplayName;
            return participant;
        }
    }
}