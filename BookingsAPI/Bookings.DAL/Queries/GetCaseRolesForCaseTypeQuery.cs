using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetCaseTypeQuery : IQuery
    {
        public GetCaseTypeQuery(string caseTypeName)
        {
            CaseTypeName = caseTypeName;
        }

        public string CaseTypeName { get; set; }
    }
    
    public class GetCaseTypeQueryHandler : IQueryHandler<GetCaseTypeQuery, CaseType>
    {
        private readonly BookingsDbContext _context;

        public GetCaseTypeQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<CaseType> Handle(GetCaseTypeQuery query)
        {
            var caseTypes = await _context.CaseTypes
                .Include(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x => x.UserRole)
                .Include(x => x.HearingTypes)
                .SingleOrDefaultAsync(x => x.Name == query.CaseTypeName);

            if (caseTypes?.CaseRoles != null && caseTypes.CaseRoles.Any())
            {
                caseTypes.CaseRoles = caseTypes?.CaseRoles?.OrderBy(x => x.Name).ToList();
                foreach (var caseRole in caseTypes.CaseRoles)
                {
                    caseRole.HearingRoles = caseRole.HearingRoles.OrderBy(x => x.Name).ToList();
                }
            }

            return caseTypes;
        }
    }
}