using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class RemoveHearingCommand : ICommand
    {
        public RemoveHearingCommand(Guid hearingId)
        {
            HearingId = hearingId;
        }

        public Guid HearingId { get; set; }
    }

    public class RemoveHearingCommandHandler : ICommandHandler<RemoveHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public RemoveHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include("HearingCases.Case")
                .Include("Participants.Person")
                .Include("Participants.Person.Address")
                .Include("Participants.Person.Organisation")
                .Include("Participants.Questionnaire")
                .Include("Participants.Questionnaire.SuitabilityAnswers")
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            
            _context.RemoveRange(hearing.GetCases());
            _context.Remove(hearing);

            var persons = hearing.Participants.Select(x => x.Person).ToList();
            var organisations = persons.Where(p => p.Organisation != null).Select(x => x.Organisation).ToList();
            var addresses = persons.Where(p => p.Address != null).Select(x => x.Address).ToList();
            
            _context.RemoveRange(organisations);
            _context.RemoveRange(addresses);
            _context.RemoveRange(persons);

            await _context.SaveChangesAsync();
        }

    }
}