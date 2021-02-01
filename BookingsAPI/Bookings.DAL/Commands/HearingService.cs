using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Dtos;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Participants;
using Bookings.Domain.Validations;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public interface IHearingService
    {
        /// <summary>
        /// Add a participant to a hearing service and returns a list of all participants.
        /// This will re-use existing personnel entries before attempting to create a new one.
        /// </summary>
        /// <param name="hearing">Hearing to amend</param>
        /// <param name="participants">List of participants to add</param>
        /// <returns></returns>
        Task<List<Participant>> AddParticipantToService(VideoHearing hearing, List<NewParticipant> participants);

        /// <summary>
        /// Update the case name of a hearing directly
        /// </summary>
        /// <param name="hearingId">Id of hearing</param>
        /// <param name="caseName">New case name</param>
        /// <returns></returns>
        Task UpdateHearingCaseName(Guid hearingId, string caseName);
        
        /// <summary>
        /// Create links between participants if any exist
        /// </summary>
        /// <param name="participants">List of participants in the hearing</param>
        /// <param name="requests">List of requests containing information of any participant links</param>
        /// <returns></returns>
        Task<List<LinkedParticipant>> CreateParticipantLinks(List<Participant> participants, List<LinkedParticipantDto> linkedParticipantDtos);
    }
    public class HearingService : IHearingService
    {
        private readonly BookingsDbContext _context;

        public HearingService(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Participant>> AddParticipantToService(VideoHearing hearing, List<NewParticipant> participants)
        {
            var participantList = new List<Participant>();
            foreach (var participantToAdd in participants)
            {
                var existingPerson = await _context.Persons
                    .Include("Organisation")
                    .SingleOrDefaultAsync(x => x.Username == participantToAdd.Person.Username);
                
                switch (participantToAdd.HearingRole.UserRole.Name)
                {
                    case "Individual":
                        var individual = hearing.AddIndividual(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName);
            
                        UpdateOrganisationDetails(participantToAdd.Person, individual);
                        participantList.Add(individual);
                        break;
                    case "Representative":
                    {
                        var representative = hearing.AddRepresentative(existingPerson ?? participantToAdd.Person,
                            participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName,
                            participantToAdd.Representee);

                        UpdateOrganisationDetails(participantToAdd.Person, representative);
                        participantList.Add(representative);
                        break;
                    }
                    case "Judicial Office Holder":
                    {
                        var joh = hearing.AddJudicialOfficeHolder(existingPerson ?? participantToAdd.Person,
                            participantToAdd.HearingRole, participantToAdd.CaseRole, participantToAdd.DisplayName);

                        participantList.Add(joh);
                        break;
                    }
                    case "Judge":
                    {
                        var judge = hearing.AddJudge(existingPerson ?? participantToAdd.Person,
                            participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName);

                        participantList.Add(judge);
                        break;
                    }
                    default:
                        throw new DomainRuleException(nameof(participantToAdd.HearingRole.UserRole.Name),
                            $"Role {participantToAdd.HearingRole.UserRole.Name} not recognised");
                }
            }

            return participantList;
        }

        public async Task UpdateHearingCaseName(Guid hearingId, string caseName)
        {
            var hearing = await _context.VideoHearings.Include(x => x.HearingCases).ThenInclude(h => h.Case)
                .FirstAsync(x => x.Id == hearingId);
            var existingCase = hearing.GetCases().First();
            hearing.UpdateCase(new Case(existingCase.Number, caseName)
            {
                IsLeadCase = existingCase.IsLeadCase
            });
            await _context.SaveChangesAsync();
        }

        public Task<List<LinkedParticipant>> CreateParticipantLinks(List<Participant> participants, List<LinkedParticipantDto> linkedParticipantDtos)
        {
            var linkedParticipants = new List<LinkedParticipant>();

            foreach (var linkedParticipantDto in linkedParticipantDtos)
            {
                var interpretee =
                    participants.Single(x => 
                        x.Person.ContactEmail.Equals(linkedParticipantDto.ParticipantContactEmail,
                            StringComparison.CurrentCultureIgnoreCase));
                var interpreter =
                    participants.Single(x =>
                        x.Person.ContactEmail.Equals(linkedParticipantDto.LinkedParticipantContactEmail,
                            StringComparison.CurrentCultureIgnoreCase));
                
                var linkedParticipant = new LinkedParticipant(interpretee.Id, interpreter.Id,
                    linkedParticipantDto.Type);

                linkedParticipants.Add(linkedParticipant);

                UpdateParticipantsWithLinks(interpretee, interpreter, linkedParticipantDto.Type);
            }

            return Task.FromResult(linkedParticipants);
        }

        private void UpdateParticipantsWithLinks(Participant interpretee, Participant interpreter, LinkedParticipantType linkType)
        {
            interpretee.AddLink(interpreter.Id, linkType);
            interpreter.AddLink(interpretee.Id, linkType);
        }

        private void UpdateOrganisationDetails(Person newPersonDetails, Participant participantToUpdate)
        {
            var newOrganisation = newPersonDetails.Organisation;
            var existingPerson = participantToUpdate.Person;
            participantToUpdate.UpdateParticipantDetails(existingPerson.Title, participantToUpdate.DisplayName, existingPerson.TelephoneNumber, newOrganisation?.Name);
        }
    }
}