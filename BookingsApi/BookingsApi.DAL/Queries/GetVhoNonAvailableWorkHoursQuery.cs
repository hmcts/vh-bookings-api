using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
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
            => await _context.VhoNonAvailabilities.Include(x => x.JusticeUser)
                                                  .Where(x => x.JusticeUser.Username == query.UserName)
                                                  .ToListAsync();
    }
}
