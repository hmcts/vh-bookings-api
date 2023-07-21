using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetAnonymisationDataQuery : IQuery
    {
    }

    public class GetAnonymisationDataQueryHandler : IQueryHandler<GetAnonymisationDataQuery, AnonymisationDataDto>
    {
        private readonly BookingsDbContext _context;
        public GetAnonymisationDataQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<AnonymisationDataDto> Handle(GetAnonymisationDataQuery query)
        {
            var cutOffDate = DateTime.UtcNow.AddMonths(-3);
            
            var lastRunDate = BaseQueries.JobHistory.GetLastRunDate(_context, SchedulerJobsNames.AnonymiseHearings);

            var cutOffDateFrom = lastRunDate.HasValue
                ? cutOffDate.AddDays((lastRunDate.Value - DateTime.UtcNow).Days - 1)
                : cutOffDate.AddDays(-30);

            var personsInFutureHearings = _context.Participants
                .Include(p => p.Person)
                .Where(h => h.Hearing.ScheduledDateTime >= cutOffDate)
                .Select(p => p.Person.Username).Distinct();

            var participants = await _context.Participants
                .Include(p => p.Person)
                .Where(h => (h.Hearing.ScheduledDateTime >= cutOffDateFrom
                             && h.Hearing.ScheduledDateTime < cutOffDate))
                .Where(p => !personsInFutureHearings.Any(pf => pf == p.Person.Username))
                .Where(p => !p.Person.Username.ToLower().EndsWith("@email.net"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("auto_vw"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("auto_sw"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("auto_tw"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("auto_aw"))
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