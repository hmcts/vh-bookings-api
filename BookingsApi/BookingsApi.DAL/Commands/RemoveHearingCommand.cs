namespace BookingsApi.DAL.Commands
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
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .Include(x=> x.JudiciaryParticipants).ThenInclude(x=> x.JudiciaryPerson)
                .Where(x => x.Id == command.HearingId || x.SourceId == command.HearingId).ToListAsync();

            if (!hearingsIncCloned.Any())
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            _context.RemoveRange(hearingsIncCloned.SelectMany(h => h.GetEndpoints()));
            _context.RemoveRange(hearingsIncCloned.SelectMany(h => h.GetCases()));
            _context.RemoveRange(hearingsIncCloned.SelectMany(h => h.Participants.SelectMany(p => p.LinkedParticipants)));

            var persons = await GetPersonsToRemove(hearingsIncCloned);
            var organisations = persons.Where(p => p.Organisation != null).Select(x => x.Organisation).ToList();

            _context.RemoveRange(organisations);
            _context.RemoveRange(persons);
            _context.RemoveRange(hearingsIncCloned);

            await _context.SaveChangesAsync();
        }

        private async Task<List<Person>> GetPersonsToRemove(List<VideoHearing> hearingsIncCloned)
        {
            var removePersons = new List<Person>();
            var distinctPersons = hearingsIncCloned.SelectMany(h => h.Participants.Select(x => x.Person)).Distinct().ToList();
            foreach (var person in distinctPersons)
            {
                var hearingIdsForPerson = await _context.Participants.Where(x => x.PersonId == person.Id).Select(x=> x.HearingId).ToListAsync();
                var hearingIds = hearingsIncCloned.Select(x => x.Id).ToList();
                // if all hearings for a person are being removed then remove the person
                if (hearingIdsForPerson.TrueForAll(hearingIds.Contains))
                {
                    removePersons.Add(person);
                }
            }
            return removePersons;
        }
    }
}