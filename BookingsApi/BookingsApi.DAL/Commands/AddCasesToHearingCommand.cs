using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class AddCasesToHearingCommand : ICommand
    {
        public List<Case> Cases { get; }
        public Guid HearingId { get; }

        public AddCasesToHearingCommand(Guid hearingId, List<Case> cases)
        {
            Cases = cases;
            HearingId = hearingId;
        }
    }

    public class AddCasesToHearingCommandHandler : ICommandHandler<AddCasesToHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddCasesToHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddCasesToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include("HearingCases.Case")
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            hearing.AddCases(command.Cases);
            await _context.SaveChangesAsync();
        }
    }
}