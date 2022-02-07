using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetUsernamesToAnonymiseQuery : IQuery
    {
        
    }

    public class GetUsernamesToAnonymiseQueryHandler : IQueryHandler<GetUsernamesToAnonymiseQuery, List<string>>
    {
        private readonly BookingsDbContext _context;

        public GetUsernamesToAnonymiseQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<string>> Handle(GetUsernamesToAnonymiseQuery query)
        {
            var cutOffDate = DateTime.UtcNow.AddMonths(-3);
            
            var lastRunDate = _context.JobHistory.FirstOrDefault()?.LastRunDate;
            
            var cutOffDateFrom = lastRunDate.HasValue 
                ? cutOffDate.AddDays((lastRunDate.Value - DateTime.UtcNow).Days - 1) 
                : cutOffDate.AddDays(-30);
            
            var personsInFutureHearings = _context.Participants
                .Include(p => p.Person)
                .Where(h => h.Hearing.ScheduledDateTime >= cutOffDate)
                .Select(p => p.Person.Username).Distinct();
            
            
            var usernames = await  _context.Participants
                .Include(p => p.Person)
                .Where(h => (h.Hearing.ScheduledDateTime >= cutOffDateFrom
                             && h.Hearing.ScheduledDateTime < cutOffDate))
                .Where(p => !personsInFutureHearings.Any(pf => pf == p.Person.Username))
                .Where(p => !p.Person.Username.ToLower().EndsWith("@email.net"))
                .Select(x => x.Person.Username).Distinct()
                .ToListAsync();
            
            return usernames;
        }
    }
}