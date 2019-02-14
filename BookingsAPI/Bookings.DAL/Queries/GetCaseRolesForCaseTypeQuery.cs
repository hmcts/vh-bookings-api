using System.Linq;
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

        public CaseType Handle(GetCaseTypeQuery query)
        {
        return _context.CaseTypes
                .Include(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x => x.UserRole)
                .Include(x => x.HearingTypes)
                .SingleOrDefault(x => x.Name == query.CaseTypeName);
        }
    }
}