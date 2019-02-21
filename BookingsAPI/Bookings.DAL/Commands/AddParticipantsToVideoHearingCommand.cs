using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.DAL.Exceptions;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class AddParticipantsToVideoHearingCommand : ICommand
    {
        public AddParticipantsToVideoHearingCommand(Guid hearingId, List<Participant> participants)
        {
            HearingId = hearingId;
            Participants = participants;
        }

        public List<Participant> Participants { get; set; }
        public Guid HearingId { get; set; }
    }
    
    public class AddParticipantsToVideoHearingCommandHandler : ICommandHandler<AddParticipantsToVideoHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddParticipantsToVideoHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddParticipantsToVideoHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x => x.Person)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            foreach (var participant in command.Participants)
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
            
            hearing.AddParticipants(command.Participants);
            await _context.SaveChangesAsync();
        }
    }
}