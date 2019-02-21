using System.Threading.Tasks;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class SaveVideoHearingCommand : ICommand
    {
        public SaveVideoHearingCommand(VideoHearing videoHearing)
        {
            VideoHearing = videoHearing;
        }

        public VideoHearing VideoHearing { get; set; }
    }

    public class SaveVideoHearingCommandHandler : ICommandHandler<SaveVideoHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public SaveVideoHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(SaveVideoHearingCommand command)
        {
            var participants = command.VideoHearing.GetParticipants();
            foreach (var participant in participants)
            {
                var existingPerson = await _context.Persons
                    .Include(x => x.Address)
                    .Include(x => x.Organisation)
                    .SingleOrDefaultAsync(x => x.Username == participant.Person.Username);

                if (existingPerson != null)
                {
                    participant.UpdatePersonDetails(existingPerson);
                }
            }
            
            _context.VideoHearings.Add(command.VideoHearing);
            await _context.SaveChangesAsync();
        }
    }
}