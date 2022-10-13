using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetVhoWorkHoursQuery : IQuery
    {
        public string UserName { get; }

        public GetVhoWorkHoursQuery(string userName) => UserName = userName.ToLowerInvariant();
        
    }

    public class GetVhoWorkHoursQueryHandler : IQueryHandler<GetVhoWorkHoursQuery, List<VhoWorkHours>>
    {
        private readonly BookingsDbContext _context;

        public GetVhoWorkHoursQueryHandler(BookingsDbContext context) => _context = context;
        
        public async Task<List<VhoWorkHours>> Handle(GetVhoWorkHoursQuery query)
            => await _context.VhoWorkHours.Include(x => x.JusticeUser)
                                          .Where(x => x.JusticeUser.Username == query.UserName)
                                          .ToListAsync();
    }
}
