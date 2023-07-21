using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetVhoNonAvailableWorkHoursQuery : IQuery
    {
        public string UserName { get; }

        public GetVhoNonAvailableWorkHoursQuery(string userName) => UserName = userName.ToLowerInvariant();
        
    }

    public class GetVhoNonAvailableWorkHoursQueryHandler : IQueryHandler<GetVhoNonAvailableWorkHoursQuery, List<VhoNonAvailability>>
    {
        private readonly BookingsDbContext _context;

        public GetVhoNonAvailableWorkHoursQueryHandler(BookingsDbContext context) => _context = context;

        public async Task<List<VhoNonAvailability>> Handle(GetVhoNonAvailableWorkHoursQuery query)
        {
            var justiceUser = await _context.JusticeUsers
                .Include(e => e.VhoNonAvailability)
                .FirstOrDefaultAsync(e => e.Username == query.UserName);
            
            if(justiceUser == null)    
                return null;

            return justiceUser.VhoNonAvailability.Where(x => x.Deleted != true).ToList() ?? new List<VhoNonAvailability>();
        }
    }
}
