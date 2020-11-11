using Bookings.DAL.Queries.Core;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.DAL.Queries
{
    public class GetPersonsByClosedHearingsQuery : IQuery
    {
        public GetPersonsByClosedHearingsQuery() { }
    }

    public class GetPersonsByClosedHearingsQueryHandler : IQueryHandler<GetPersonsByClosedHearingsQuery, List<string>>
    {
        private readonly BookingsDbContext _context;
        public GetPersonsByClosedHearingsQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> Handle(GetPersonsByClosedHearingsQuery query)
        {
            var cutOffDate = DateTime.UtcNow.AddMonths(-3);

            var lastRunDate = _context.JobHistory.FirstOrDefault()?.LastRunDate;

            var cutOffDateFrom = lastRunDate.HasValue 
                                        ? cutOffDate.AddDays((lastRunDate.Value - DateTime.UtcNow).Days - 1) 
                                        : cutOffDate.AddDays(-30);

            var personsInFutureHearings = _context.Participants
                .Include(p => p.Person)
                .Include(p => p.HearingRole).ThenInclude(h => h.UserRole)
                .Where(h => h.Hearing.ScheduledDateTime >= cutOffDate
                            && (h.HearingRole.UserRole.Name.ToLower().Equals("individual")
                            || h.HearingRole.UserRole.Name.ToLower().Equals("representative")))
                .Select(p => p.Person.Username).Distinct();
            
            var personsInPastHearings = await _context.Participants
                .Include(p => p.Person)
                .Include(p => p.HearingRole).ThenInclude(h => h.UserRole)
                .Where(h => (h.Hearing.ScheduledDateTime >= cutOffDateFrom
                            && h.Hearing.ScheduledDateTime < cutOffDate)
                            && (h.HearingRole.UserRole.Name.ToLower().Equals("individual")
                            || h.HearingRole.UserRole.Name.ToLower().Equals("representative")))
                .Where(p => !personsInFutureHearings.Any( pf => pf == p.Person.Username))
                .Where(p => !p.Person.Username.ToLower().EndsWith("@email.net"))
                .Where(p => !p.Person.Username.Contains("atif."))
                .Where(p => !p.Person.Username.ToLower().StartsWith("ferdinand.porsche"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("enzo.ferrari"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("mike.tyson"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("george.foreman"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("rocky.marciano"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("cassius.clay"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("george.clinton"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("metalface.doom"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("karl.benz"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("henry.ford"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("feuer.frei"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("wasser.kalt"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("dan.brown"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("tom.clancy"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("stephen.king"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("sue.burke"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("michael.jordan"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("scottie.pippen"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("steve.kerr"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("dennis.rodman"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("Jane.Doe"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("John.Doe"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("Chris.Green"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("James.Green"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("yeliz.admin"))
                .Where(p => !p.Person.Username.StartsWith("Automation01"))
                .Where(p => !p.Person.Username.StartsWith("auto."))
                .Where(p => !p.Person.Username.ToLower().StartsWith("auto_")) // Used by testAPI
                .Where(p => !p.Person.Username.ToLower().StartsWith("userapitestuser"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("manual"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("ithc")) //Used by ITHC
                .Where(p => !p.Person.Username.ToLower().StartsWith("performance")) //Used for perfomance testing
                .Where(p => !p.Person.Username.ToLower().Contains("test"))
                .Where(p => !p.Person.Username.ToLower().Contains("kinly.clerk"))
                .Where(p => !p.Person.ContactEmail.ToLower().EndsWith("testusersdomain.net"))
                .Select(p => p.Person.Username).Distinct()
                .ToListAsync();

            return personsInPastHearings;
        }
    }
}
