using BookingsApi.DAL.Queries.BaseQueries;
using BookingsApi.Domain.RefData;

namespace BookingsApi.DAL.Queries
{
    public class GetCaseRolesForCaseTypeQuery : IQuery
    {
        public GetCaseRolesForCaseTypeQuery(string caseTypeQueryParameter)
        {
            CaseTypeQueryParameter = caseTypeQueryParameter;
        }

        /// <summary>
        /// Can be caseType name or ServiceId (depending on whether refData flag on/off)
        /// </summary>
        public string CaseTypeQueryParameter { get; }
    }
    
    public class GetCaseRolesForCaseTypeQueryHandler : IQueryHandler<GetCaseRolesForCaseTypeQuery, CaseType>
    {
        private readonly BookingsDbContext _context;

        public GetCaseRolesForCaseTypeQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<CaseType> Handle(GetCaseRolesForCaseTypeQuery query)
        {
            var caseTypesQuery = CaseTypes.Get(_context);
            var caseType = await caseTypesQuery.SingleOrDefaultAsync(x => x.Name == query.CaseTypeQueryParameter);
            caseType.PopulateCaseRoles();
            return caseType;
        }
    }
}