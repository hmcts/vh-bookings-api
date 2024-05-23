using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Helper;

namespace BookingsApi.DAL.Queries
{
    public class GetAnonymisationDataQuery : IQuery { }

    public class GetAnonymisationDataQueryHandler : IQueryHandler<GetAnonymisationDataQuery, AnonymisationDataDto>
    {
        private readonly BookingsDbContext _context;
        public GetAnonymisationDataQueryHandler(BookingsDbContext context) =>_context = context;
        public async Task<AnonymisationDataDto> Handle(GetAnonymisationDataQuery query)
        {
            //All hearings that have been scheduled before the last 3 months to be anonymised
            var cutOffDate = DateTime.UtcNow.AddMonths(-3);
            
            //The last time this job ran successfully
            var lastRunDate = BaseQueries.JobHistory.GetLastRunDate(_context, SchedulerJobsNames.AnonymiseHearings);

            //the date after which all hearings should be anonymised
            var cutOffDateFrom = lastRunDate.HasValue
                ? cutOffDate.AddDays((lastRunDate.Value - DateTime.UtcNow).Days - 1)
                : DateTime.MinValue; //Run from the beginning of time, if the job has never run successfully before

            //Persons that should be ignored from anonymisation due to being scheduled on future hearings (after the cutoff date)
            var personsInFutureHearings = _context.Participants
                .Include(p => p.Person)
                .Where(h => h.Hearing.ScheduledDateTime >= cutOffDate)
                .Select(p => p.Person.Username)
                .Distinct()
                .AsQueryable();

            var participants = await _context.Participants
                .Include(p => p.Person)
                .Where(h => h.Hearing.ScheduledDateTime >= cutOffDateFrom && h.Hearing.ScheduledDateTime < cutOffDate)
                .Where(p => personsInFutureHearings.All(pf => pf != p.Person.Username))
                .Where(p => p.Person.Username.ToLower().EndsWith("@hearings.reform.hmcts.net") // created in ad
                            || p.Person.Username.ToLower().Equals(p.Person.ContactEmail.ToLower()) // failed to create in ad
                            || p.Person.ContactEmail == null) // never got a contact email to complete the user creation
                .Where(p => !p.Person.Username.ToLower().StartsWith("auto_"))
                .ToListAsync();

            var usernameDto = new AnonymisationDataDto
            {
                Usernames = participants.Select(x => x.Person.Username).Distinct().ToList(),
                HearingIds = participants.Select(x => x.HearingId).Distinct().ToList()
            };

            return usernameDto;
        }
    }
}