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
                .Where(h => h.Hearing.ScheduledDateTime < cutOffDate
                            && (h.HearingRole.UserRole.Name.ToLower().Equals("individual")
                            || h.HearingRole.UserRole.Name.ToLower().Equals("representative")))
                .Where(p => !personsInFutureHearings.Any( pf => pf == p.Person.Username))
                .Where(p => !p.Person.Username.Contains("@email.net"))
                
                .Where(p => !p.Person.Username.Contains("atif."))
                .Where(p => !p.Person.Username.Contains("y''test."))
                .Where(p => !p.Person.Username.Contains("ferdinand.porsche"))
                .Where(p => !p.Person.Username.Contains("enzo.ferrari"))
                .Where(p => !p.Person.Username.Contains("mike.tyson"))
                .Where(p => !p.Person.Username.Contains("george.foreman"))
                .Where(p => !p.Person.Username.Contains("rocky.marciano"))
                .Where(p => !p.Person.Username.Contains("cassius.clay"))
                .Where(p => !p.Person.Username.Contains("george.clinton"))
                .Where(p => !p.Person.Username.Contains("metalface.doom"))
                .Where(p => !p.Person.Username.Contains("karl.benz"))
                .Where(p => !p.Person.Username.Contains("henry.ford"))
                .Where(p => !p.Person.Username.Contains("feuer.frei"))
                .Where(p => !p.Person.Username.Contains("wasser.kalt"))
                .Where(p => !p.Person.Username.Contains("dan.brown"))
                .Where(p => !p.Person.Username.Contains("tom.clancy"))
                .Where(p => !p.Person.Username.Contains("stephen.king"))
                .Where(p => !p.Person.Username.Contains("sue.burke"))
                .Where(p => !p.Person.Username.Contains("michael.jordan"))
                .Where(p => !p.Person.Username.Contains("scottie.pippen"))
                .Where(p => !p.Person.Username.Contains("steve.kerr"))
                .Where(p => !p.Person.Username.Contains("dennis.rodman"))
                .Where(p => !p.Person.Username.Contains("Jane.Doe"))
                .Where(p => !p.Person.Username.Contains("John.Doe"))
                .Where(p => !p.Person.Username.Contains("Chris.Green"))
                .Where(p => !p.Person.Username.Contains("James.Green"))
                .Where(p => !p.Person.Username.Contains("yeliz.admin"))
                .Where(p => !p.Person.Username.ToLower().StartsWith("auto")) // Used by testAPI
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
