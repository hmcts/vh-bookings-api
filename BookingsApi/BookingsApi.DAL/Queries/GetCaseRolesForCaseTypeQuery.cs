using BookingsApi.DAL.Queries.BaseQueries;

namespace BookingsApi.DAL.Queries
{
    public class GetCaseRolesForCaseTypeQuery(string caseTypeQueryParameter) : IQuery
    {
        /// <summary>
        /// Can be caseType name or ServiceId (depending on whether refData flag on/off)
        /// </summary>
        public string CaseTypeQueryParameter { get; } = caseTypeQueryParameter;
    }
    
    public class GetCaseRolesForCaseTypeQueryHandler(BookingsDbContext context)
        : IQueryHandler<GetCaseRolesForCaseTypeQuery, CaseType>
    {
        public async Task<CaseType> Handle(GetCaseRolesForCaseTypeQuery query)
        {
            var caseTypesQuery = CaseTypes.Get(context);
            var caseType = await caseTypesQuery.SingleOrDefaultAsync(x => x.Name == query.CaseTypeQueryParameter);        
            if (caseType?.CaseRoles != null && caseType.CaseRoles.Count != 0)
                caseType.PopulateCaseRoles();
            return caseType;
        }
    }
}