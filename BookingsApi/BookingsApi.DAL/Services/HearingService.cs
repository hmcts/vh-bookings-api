using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.DAL.Services
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
        /// <param name="linkedParticipantDtos">List linked participants dtos containing the linking data</param>
        /// <returns></returns>
        Task CreateParticipantLinks(List<Participant> participants, List<LinkedParticipantDto> linkedParticipantDtos);

        /// <summary>
        /// Removes the link between participants
        /// </summary>
        /// <param name="participants">All participants in a hearing</param>
        /// <param name="participant">The participants with whom to remove links</param>
        /// <returns></returns>
        Task RemoveParticipantLinks(List<Participant> participants, Participant participant);

        Task AddJudiciaryParticipantToVideoHearing(VideoHearing videoHearing, NewJudiciaryParticipant participant);

        Task ReassignJudge(VideoHearing hearing, NewParticipant newJudgeParticipant);
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
                    .SingleOrDefaultAsync(x => x.ContactEmail == participantToAdd.Person.ContactEmail);

                if(existingPerson != null)
                {
                    var person = participantToAdd.Person;
                    existingPerson.UpdatePerson(person.FirstName, person.LastName, person.Title, person.TelephoneNumber);
                }

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
                                    participantToAdd.CaseRole, 
                                    participantToAdd.DisplayName);

                            participantList.Add(judge);
                            break;
                        }
                    default:
                        throw new DomainRuleException(nameof(participantToAdd.HearingRole.UserRole.Name),
                            $"Role {participantToAdd.HearingRole.UserRole.Name} not recognised");
                }
            }
            await LoadHearingRoles(participantList);
            return participantList;
        }

        public async Task ReassignJudge(VideoHearing hearing, NewParticipant newJudgeParticipant)
        {
            var person = await _context.Persons.FirstAsync(x => x.ContactEmail == newJudgeParticipant.Person.ContactEmail);
            var judge = new Judge(person, newJudgeParticipant.HearingRole, newJudgeParticipant.CaseRole);
            hearing.ReassignJudge(judge);
        }

        private async Task LoadHearingRoles(List<Participant> participantList)
        {
            foreach (var participant in participantList)
            {
                participant.HearingRole = await _context.HearingRoles
                    .Include(h => h.UserRole)
                    .FirstOrDefaultAsync(x => x.Id == participant.HearingRoleId);
            }
        }
        
        public async Task UpdateHearingCaseName(Guid hearingId, string caseName)
        {
            var hearing = await _context.VideoHearings.Include(x => x.HearingCases).ThenInclude(h => h.Case)
                .FirstAsync(x => x.Id == hearingId);
            var existingCase = hearing.GetCases()[0];
            hearing.UpdateCase(new Case(existingCase.Number, caseName)
            {
                IsLeadCase = existingCase.IsLeadCase
            });
            await _context.SaveChangesAsync();
        }

        public Task CreateParticipantLinks(List<Participant> participants, List<LinkedParticipantDto> linkedParticipantDtos)
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

        public Task RemoveParticipantLinks(List<Participant> participants, Participant participant)
        {
            var linkedParticipants = participant.LinkedParticipants.Select(l => new LinkedParticipant(l.ParticipantId, l.LinkedId, l.Type)).ToList();
            foreach (var lp in linkedParticipants)
            {
                var interpreter = participants.Single(x => x.Id == lp.ParticipantId);
                var interpretee = participants.Single(x => x.Id == lp.LinkedId);

                var lp1 = interpreter.LinkedParticipants.Single(x => x.LinkedId == interpretee.Id);
                var lp2 = interpretee.LinkedParticipants.Single(x => x.LinkedId == interpreter.Id);

                interpretee.RemoveLink(lp2);
                interpreter.RemoveLink(lp1);
            }
            return Task.CompletedTask;
        }

        public async Task AddJudiciaryParticipantToVideoHearing(VideoHearing videoHearing, NewJudiciaryParticipant participant)
        {
            var judiciaryPerson = await _context.JudiciaryPersons
                .SingleOrDefaultAsync(x => x.PersonalCode == participant.PersonalCode);

            if (judiciaryPerson == null)
                throw new JudiciaryPersonNotFoundException(participant.PersonalCode);

            switch (participant.HearingRoleCode)
            {
                case JudiciaryParticipantHearingRoleCode.Judge:
                    videoHearing.AddJudiciaryJudge(judiciaryPerson, participant.DisplayName, participant.OptionalContactEmail, participant.OptionalContactTelephone);
                    break;
                case JudiciaryParticipantHearingRoleCode.PanelMember:
                    videoHearing.AddJudiciaryPanelMember(judiciaryPerson, participant.DisplayName);
                    break;
                default:
                    throw new ArgumentException($"Role {participant.HearingRoleCode} not recognised");
            }
        }

        private static void UpdateParticipantsWithLinks(Participant participant1, Participant participant2, LinkedParticipantType linkType)
        {
            participant1.AddLink(participant2.Id, linkType);
            participant2.AddLink(participant1.Id, linkType);
        }

        private static void UpdateOrganisationDetails(Person newPersonDetails, Participant participantToUpdate)
        {
            var newOrganisation = newPersonDetails.Organisation;
            var existingPerson = participantToUpdate.Person;
            participantToUpdate.UpdateParticipantDetails(existingPerson.Title, participantToUpdate.DisplayName, newPersonDetails.TelephoneNumber, newOrganisation?.Name);
        }
    }
}