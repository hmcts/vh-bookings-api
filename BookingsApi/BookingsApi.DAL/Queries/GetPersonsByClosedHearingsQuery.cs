using BookingsApi.Common.Helpers;
using BookingsApi.DAL.Helper;

namespace BookingsApi.DAL.Queries;

public class GetPersonsByClosedHearingsQuery : IQuery;


#pragma warning disable CA1862 // Disabled as must be SQL compliant
public class GetPersonsByClosedHearingsQueryHandler(BookingsDbContext context)
    : IQueryHandler<GetPersonsByClosedHearingsQuery, List<string>>
{
    public async Task<List<string>> Handle(GetPersonsByClosedHearingsQuery query)
    {
        var cutOffDate = DateTime.UtcNow.AddMonths(-3);

        var lastRunDate = BaseQueries.JobHistory.GetLastRunDate(context, SchedulerJobsNames.AnonymiseHearings);

        var cutOffDateFrom = lastRunDate.HasValue 
            ? cutOffDate.AddDays((lastRunDate.Value - DateTime.UtcNow).Days - 1) 
            : cutOffDate.AddDays(-30);

        var personsInFutureHearings = context.Participants
            .Include(p => p.Person)
            .Include(p => p.HearingRole).ThenInclude(h => h.UserRole)
            .Where(h => h.Hearing.ScheduledDateTime >= cutOffDate
                        && (!h.HearingRole.UserRole.Name.ToLower().Equals("judge")))
            .Select(p => p.Person.Username).Distinct();
            
        var personsInPastHearings = await context.Participants
            .Include(p => p.Person)
            .Include(p => p.HearingRole).ThenInclude(h => h.UserRole)
            .Where(h => (h.Hearing.ScheduledDateTime >= cutOffDateFrom
                         && h.Hearing.ScheduledDateTime < cutOffDate)
                        && (!h.HearingRole.UserRole.Name.ToLower().Equals("judge")))
            .Where(p => !personsInFutureHearings.Any( pf => pf == p.Person.Username))
            .Where(p => !p.Person.Username.ToLower().EndsWith("@email.net"))
            .Where(p => !p.Person.Username.Contains("atif."))

            .Where(p => !p.Person.Username.ToLower().StartsWith("Test"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("TP"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("Auto_"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("CACD"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("Employment"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("GRC"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("IAC"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("Judge"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("Property"))

            .Where(p => !p.Person.Username.ToLower().StartsWith("yeliz.admin"))
            .Where(p => !p.Person.Username.StartsWith("Automation01"))
            .Where(p => !p.Person.Username.StartsWith("auto."))
            .Where(p => !p.Person.Username.ToLower().StartsWith("auto_")) // Used by testAPI
            .Where(p => !p.Person.Username.ToLower().StartsWith("userapitestuser"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("manual"))
            .Where(p => !p.Person.Username.ToLower().StartsWith("ithc")) //Used by ITHC
            .Where(p => !p.Person.Username.ToLower().StartsWith("performance")) //Used for perfomance testing
            .Where(p => !p.Person.Username.ToLower().Contains("test"))
            .Where(p => !p.Person.ContactEmail.ToLower().EndsWith("testusersdomain.net"))
            .Select(p => p.Person.Username).Distinct()
            .ToListAsync();

        return personsInPastHearings;
    }
}
#pragma warning restore CA1862