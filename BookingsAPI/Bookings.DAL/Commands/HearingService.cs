using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.Validations;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public interface IHearingService
    {
        Task AddParticipantToService(VideoHearing hearing, List<NewParticipant> participants);
    }
    public class HearingService : IHearingService
    {
        private readonly BookingsDbContext _context;

        public HearingService(BookingsDbContext context)
        {
            _context = context;
        }
        public async Task AddParticipantToService(VideoHearing hearing, List<NewParticipant> participants)
        {
            // for each participant
            // check for existing person
            // add new individual/solicitor
            foreach (var participantToAdd in participants)
            {
                var existingPerson = await _context.Persons
                    .Include("Address")
                    .Include("Organisation")
                    .SingleOrDefaultAsync(x => x.Username == participantToAdd.Person.Username);

                if (existingPerson == null)
                {
                    _context.Entry(participantToAdd.Person).State = EntityState.Added;
                }
                switch (participantToAdd.HearingRole.UserRole.Name)
                {
                    case "Individual":
                        var individual = hearing.AddIndividual(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName);
                        _context.Entry(individual).State = EntityState.Added;
                        UpdateAddressAndOrganisationDetails(participantToAdd.Person, individual);
                        break;
                    case "Representative":
                    {
                            var solicitor = hearing.AddSolicitor(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName,
                            participantToAdd.SolicitorsReference, participantToAdd.Representee);
                            _context.Entry(solicitor).State = EntityState.Added;
                            UpdateAddressAndOrganisationDetails(participantToAdd.Person, solicitor);
                        break;
                    }
                    case "Judge":
                        var judge = hearing.AddJudge(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName);
                        _context.Entry(judge).State = EntityState.Added;
                        break;
                    default:
                        throw new DomainRuleException(nameof(participantToAdd.HearingRole.UserRole.Name),
                            $"Role {participantToAdd.HearingRole.UserRole.Name} not recognised");
                }
            }
        }
        private void UpdateAddressAndOrganisationDetails(Person newPersonDetails, Participant participantToUpdate)
        {
            var newAddress = newPersonDetails.Address;
            var newOrganisation = newPersonDetails.Organisation;
            var existingPerson = participantToUpdate.Person;
            participantToUpdate.UpdateParticipantDetails(existingPerson.Title, participantToUpdate.DisplayName, existingPerson.TelephoneNumber,
                   newAddress?.Street, newAddress?.HouseNumber, newAddress?.City, newAddress?.County, newAddress?.Postcode, newOrganisation?.Name);
        }
    }
}