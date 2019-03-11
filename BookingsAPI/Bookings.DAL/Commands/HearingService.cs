using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.Domain;
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
                    .SingleOrDefaultAsync(x => x.Username == participantToAdd.Person.Username);
                
                switch (participantToAdd.HearingRole.UserRole.Name)
                {
                    case "Individual":
                        hearing.AddIndividual(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName);
                        break;
                    case "Representative":
                    {
                        hearing.AddSolicitor(existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole,
                            participantToAdd.CaseRole, participantToAdd.DisplayName,
                            participantToAdd.SolicitorsReference, participantToAdd.Representee);
                        break;
                    }
                    default:
                        throw new DomainRuleException(nameof(participantToAdd.HearingRole.UserRole.Name),
                            $"Role {participantToAdd.HearingRole.UserRole.Name} not recognised");
                }
            }
        }
    }
}