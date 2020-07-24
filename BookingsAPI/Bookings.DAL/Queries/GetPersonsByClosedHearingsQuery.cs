using Bookings.DAL.Queries.Core;
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
            var personsInFutureHearings = await _context.Participants
                .Include("Person")
                .Include("HearingRole")
                .Include("Hearing.HearingCases.Case")
                .Where(h => h.Hearing.ScheduledDateTime >= cutOffDate)
                .Select(p => p.Person.Username)
                .ToListAsync();

            var personsInPastHearings = await _context.Participants
                .Include("Person")
                .Include("HearingRole")
                .Include("Hearing.HearingCases.Case")
                .Where(h => h.Hearing.ScheduledDateTime < cutOffDate)
                .Where(p => !personsInFutureHearings.Contains(p.Person.Username))

                .Where(p => !p.Person.Username.Contains("Manual"))
                .Where(p => !p.Person.Username.Contains("JUDGE"))
                .Where(p => !p.Person.Username.Contains("TaylorHousecourt"))
                .Where(p => !p.Person.Username.Contains("ManchesterCFJCcourt"))
                .Where(p => !p.Person.Username.Contains("BirminghamCFJCcourt"))
                .Where(p => !p.Person.Username.Contains("ManchesterCFJCDDJretiringroom"))
                .Where(p => !p.Person.Username.Contains("ManchesterCFJCcourtGen"))
                .Where(p => !p.Person.Username.Contains("BirminghamCFJCcourtGen"))
                .Where(p => !p.Person.Username.Contains("BirminghamCJC.Judge"))
                .Where(p => !p.Person.Username.Contains("holdingroom"))
                .Where(p => !p.Person.Username.Contains("Property.Judge"))
                .Where(p => !p.Person.Username.Contains("TaylorHousecourt"))
                .Where(p => !p.Person.Username.Contains("TaylorHousecourtGen"))
                .Where(p => !p.Person.Username.Contains("Automation01"))
                .Where(p => !p.Person.Username.Contains("auto."))
                .Where(p => !p.Person.Username.Contains("UserApiTestUser"))
                .Where(p => !p.Person.Username.Contains("Manual0"))
                .Where(p => !p.Person.Username.Contains("performance"))
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
                .Where(p => !p.Person.Username.Contains("Manual01VideoHearingsOfficer01"))
                .Where(p => !p.Person.Username.Contains("sue.burke"))
                .Where(p => !p.Person.Username.Contains("yeliz.admin"))
                .Where(p => !p.Person.Username.Contains("yeliz.judge"))
                .Where(p => !p.Person.Username.Contains("yeliz.judge2"))
                .Where(p => !p.Person.Username.Contains("one.three"))
                .Where(p => !p.Person.Username.Contains("one.four"))
                .Where(p => !p.Person.Username.Contains("michael.jordan"))
                .Where(p => !p.Person.Username.Contains("scottie.pippen"))
                .Where(p => !p.Person.Username.Contains("steve.kerr"))
                .Where(p => !p.Person.Username.Contains("dennis.rodman"))

                .Select(p => p.Person.Username)
                .ToListAsync();

            return personsInPastHearings;
        }
    }
}
