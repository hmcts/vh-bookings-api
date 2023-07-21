using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetCaseTypeQuery : IQuery
    {
        public GetCaseTypeQuery(string caseTypeQueryParameter)
        {
            CaseTypeQueryParameter = caseTypeQueryParameter;
        }

        /// <summary>
        /// Can be caseType name or ServiceId (depending on whether refData flag on/off)
        /// </summary>
        public string CaseTypeQueryParameter { get; set; }
    }
    
    public class GetCaseTypeQueryHandler : IQueryHandler<GetCaseTypeQuery, CaseType>
    {
        private readonly BookingsDbContext _context;
        private readonly IFeatureToggles _featureToggles;

        public GetCaseTypeQueryHandler(BookingsDbContext context, IFeatureToggles featureToggles)
        {
            _context = context;
            _featureToggles = featureToggles;
        }

        public async Task<CaseType> Handle(GetCaseTypeQuery query)
        {
            var caseTypesQuery = _context.CaseTypes
                .Include(x => x.CaseRoles)
                .ThenInclude(x => x.HearingRoles)
                .ThenInclude(x => x.UserRole)
                .Include(x => x.HearingTypes)
                .AsQueryable();
            
            CaseType caseTypes;
            if (_featureToggles.ReferenceDataToggle())
                caseTypes = await caseTypesQuery.SingleOrDefaultAsync(x => x.ServiceId == query.CaseTypeQueryParameter);
            else
                caseTypes = await caseTypesQuery.SingleOrDefaultAsync(x => x.Name == query.CaseTypeQueryParameter);
            
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