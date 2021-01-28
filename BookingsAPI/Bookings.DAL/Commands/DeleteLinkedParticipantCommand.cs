using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class DeleteLinkedParticipantCommand : ICommand
    {
        public DeleteLinkedParticipantCommand(Guid id)
        {
            Id = id;
        }
        
        public Guid Id { get; }
    }

    public class DeleteLinkedParticipantCommandHandler : ICommandHandler<DeleteLinkedParticipantCommand>
    {
        private readonly BookingsDbContext _context;

        public DeleteLinkedParticipantCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(DeleteLinkedParticipantCommand command)
        {
            var linkedParticipant = await _context.LinkedParticipant
                .SingleOrDefaultAsync(x => x.Id == command.Id);

            if (linkedParticipant == null)
            {
                throw new ParticipantNotFoundException(command.Id);
            }
            
            _context.LinkedParticipant.Remove(linkedParticipant);
            await _context.SaveChangesAsync();
        }
    }
}