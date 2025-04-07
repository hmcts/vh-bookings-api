namespace BookingsApi.DAL.Commands
{
    public class RemoveHearingCommand(Guid hearingId) : ICommand
    {
        public Guid HearingId { get; set; } = hearingId;
    }

    public class RemoveHearingCommandHandler(BookingsDbContext context) : ICommandHandler<RemoveHearingCommand>
    {
        public async Task Handle(RemoveHearingCommand command)
        {
            var hearingsIncCloned = await context.VideoHearings
                .Include(x => x.HearingCases).ThenInclude(x => x.Case)
                .Include(x => x.Participants).ThenInclude(x => x.Person).ThenInclude(x => x.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Endpoints).ThenInclude(x => x.ParticipantsLinked).ThenInclude(p => p.Person)
                .Include(x=> x.JudiciaryParticipants).ThenInclude(x=> x.JudiciaryPerson)
                // keep the following includes for the screening entities - cannot auto include due to cyclic dependency
                .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
                .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
                .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
                .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
                .Where(x => x.Id == command.HearingId || x.SourceId == command.HearingId).AsSplitQuery().ToListAsync();

            if (hearingsIncCloned.Count == 0)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            context.RemoveRange(hearingsIncCloned.SelectMany(x => x.Participants).Where(p => p.Screening != null)
                .Select(s => s.Screening).ToList());
            context.RemoveRange(hearingsIncCloned.SelectMany(x => x.Endpoints).Where(p => p.Screening != null)
                .Select(s => s.Screening).ToList());
            context.RemoveRange(hearingsIncCloned.SelectMany(h => h.GetEndpoints()));
            context.RemoveRange(hearingsIncCloned.SelectMany(h => h.GetCases()));
            context.RemoveRange(hearingsIncCloned.SelectMany(h => h.Participants.SelectMany(p => p.LinkedParticipants)));

            var persons = await GetPersonsToRemove(hearingsIncCloned);
            var organisations = persons.Where(p => p.Organisation != null).Select(x => x.Organisation).ToList();

            context.RemoveRange(organisations);
            context.RemoveRange(persons);
            context.RemoveRange(hearingsIncCloned);

            await context.SaveChangesAsync();
        }

        private async Task<List<Person>> GetPersonsToRemove(List<VideoHearing> hearingsIncCloned)
        {
            var removePersons = new List<Person>();
            var distinctPersons = hearingsIncCloned.SelectMany(h => h.Participants.Select(x => x.Person)).Distinct().ToList();
            foreach (var person in distinctPersons)
            {
                var hearingIdsForPerson = await context.Participants.Where(x => x.PersonId == person.Id).Select(x=> x.HearingId).ToListAsync();
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