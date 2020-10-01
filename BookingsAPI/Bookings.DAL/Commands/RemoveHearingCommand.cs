using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Castle.Core.Internal;
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
            var hearingsIncCloned = await _context.VideoHearings
                .Include("HearingCases.Case")
                .Include("Participants.Person")
                .Include("Participants.Person.Organisation")
                .Include("Participants.Questionnaire")
                .Include("Participants.Questionnaire.SuitabilityAnswers")
                .Include(x => x.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .Where(x =>  x.Id == command.HearingId || x.SourceId == command.HearingId).ToListAsync();
            
            if (hearingsIncCloned.IsNullOrEmpty())
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            
            _context.RemoveRange(hearingsIncCloned.SelectMany(h => h.GetEndpoints()));
            _context.RemoveRange(hearingsIncCloned.SelectMany(h => h.GetCases()));

            var persons = hearingsIncCloned.SelectMany(h => h.Participants.Select(x => x.Person)).ToList();
            var organisations = persons.Where(p => p.Organisation != null).Select(x => x.Organisation).ToList();
            _context.RemoveRange(organisations);
            _context.RemoveRange(persons);

            _context.RemoveRange(hearingsIncCloned);

            await _context.SaveChangesAsync();
        }

    }
}