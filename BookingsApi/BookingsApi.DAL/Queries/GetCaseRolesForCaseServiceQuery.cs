using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.BaseQueries;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetCaseRolesForCaseServiceQuery : IQuery
    {
        public GetCaseRolesForCaseServiceQuery(string serviceId)
        {
            ServiceId = serviceId;
        }

        /// <summary>
        /// Can be caseType name or ServiceId (depending on whether refData flag on/off)
        /// </summary>
        public string ServiceId { get; }
    }
    
    public class GetCaseRolesForCaseServiceQueryHandler : IQueryHandler<GetCaseRolesForCaseServiceQuery, CaseType>
    {
        private readonly BookingsDbContext _context;

        public GetCaseRolesForCaseServiceQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<CaseType> Handle(GetCaseRolesForCaseServiceQuery query)
        {
            var caseTypesQuery = CaseTypes.Get(_context);
            var caseType = await caseTypesQuery.SingleOrDefaultAsync(x => EF.Functions.Like(x.ServiceId, $"{query.ServiceId}"));
            if (caseType?.CaseRoles != null && caseType.CaseRoles.Any())
                caseType.PopulateCaseRoles();
            return caseType;
        }
    }
}