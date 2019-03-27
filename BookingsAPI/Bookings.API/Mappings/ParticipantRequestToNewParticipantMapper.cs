using System;
using System.Linq;
using Bookings.Api.Contract.Requests;
using Bookings.Common;
using Bookings.DAL.Commands;
using Bookings.Domain;
using Bookings.Domain.RefData;

namespace Bookings.API.Mappings
{
    /// <summary>
    /// This class is used to map a participant request object to the NewParticipant model
    /// used by the AddParticipantsToVideoHearingCommand.
    /// </summary>
    public class ParticipantRequestToNewParticipantMapper
    {
        public NewParticipant MapRequestToNewParticipant(ParticipantRequest requestParticipant, CaseType caseType)
        {
            var caseRole = caseType.CaseRoles.FirstOrDefault(x => x.Name == requestParticipant.CaseRoleName);
            if (caseRole == null) throw new BadRequestException($"Invalid case role [{requestParticipant.CaseRoleName}]");
            
            var hearingRole = caseRole.HearingRoles.FirstOrDefault(x => x.Name == requestParticipant.HearingRoleName);
            if (hearingRole == null) throw new BadRequestException($"Invalid hearing role [{requestParticipant.HearingRoleName}]");

            var person = new Person(requestParticipant.Title, requestParticipant.FirstName, requestParticipant.LastName,
                requestParticipant.Username)
            {
                MiddleNames = requestParticipant.MiddleNames,
                ContactEmail = requestParticipant.ContactEmail,
                TelephoneNumber = requestParticipant.TelephoneNumber
            };

            return new NewParticipant
            {
                Person = person,
                CaseRole = caseRole,
                HearingRole = hearingRole,
                DisplayName = requestParticipant.DisplayName,
                Representee = requestParticipant.Representee,
                SolicitorsReference = requestParticipant.SolicitorsReference,
            };
        }
    }
}