using BookingsApi.DAL.Queries.BaseQueries;

namespace BookingsApi.DAL.Queries
{
    public class GetCaseRolesForCaseServiceQuery(string serviceId) : IQuery
    {
        /// <summary>
        /// Can be caseType name or ServiceId (depending on whether refData flag on/off)
        /// </summary>
        public string ServiceId { get; } = serviceId;
    }
    
    public class GetCaseRolesForCaseServiceQueryHandler(BookingsDbContext context)
        : IQueryHandler<GetCaseRolesForCaseServiceQuery, CaseType>
    {
        public async Task<CaseType> Handle(GetCaseRolesForCaseServiceQuery query)
        {
            var caseTypesQuery = CaseTypes.Get(context);
            var caseType = await caseTypesQuery.SingleOrDefaultAsync(x => EF.Functions.Like(x.ServiceId, $"{query.ServiceId}"));
            if (caseType?.CaseRoles != null && caseType.CaseRoles.Count != 0)
                caseType.PopulateCaseRoles();
            return caseType;
        }
    }
}