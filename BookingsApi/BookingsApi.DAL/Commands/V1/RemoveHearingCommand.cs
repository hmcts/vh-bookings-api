using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands.V1
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
                .Include(x => x.HearingCases).ThenInclude(x => x.Case)
                .Include(x => x.Participants).ThenInclude(x => x.Person).ThenInclude(x => x.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)//.ThenInclude(x => x.Participant)
                .Include(x => x.Participants).ThenInclude(x => x.Questionnaire).ThenInclude(x => x.SuitabilityAnswers)
                .Include(x => x.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .Where(x => x.Id == command.HearingId || x.SourceId == command.HearingId).ToListAsync();

            if (hearingsIncCloned.IsNullOrEmpty())
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            _context.RemoveRange(hearingsIncCloned.SelectMany(h => h.GetEndpoints()));
            _context.RemoveRange(hearingsIncCloned.SelectMany(h => h.GetCases()));
            _context.RemoveRange(hearingsIncCloned.SelectMany(h => h.Participants.SelectMany(p => p.LinkedParticipants)));

            var persons = GetPersonsToRemove(hearingsIncCloned);
            var organisations = persons.Where(p => p.Organisation != null).Select(x => x.Organisation).ToList();

            _context.RemoveRange(organisations);
            _context.RemoveRange(persons);

            _context.RemoveRange(hearingsIncCloned);

            await _context.SaveChangesAsync();
        }

        private List<Person> GetPersonsToRemove(List<VideoHearing> hearingsIncCloned)
        {
            var removePersons = new List<Person>();
            foreach (var person in hearingsIncCloned.SelectMany(h => h.Participants.Select(x => x.Person)))
            {
                if (_context.Participants.Any(p => p.PersonId == person.Id && p.HearingId != hearingsIncCloned.FirstOrDefault().Id)) continue;
                removePersons.Add(person);
            }
            return removePersons;
        }
    }
}