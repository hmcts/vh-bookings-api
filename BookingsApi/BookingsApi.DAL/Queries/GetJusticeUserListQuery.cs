using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
{
    public class GetJusticeUserListQuery : IQuery
    {
        public GetJusticeUserListQuery()
        {
        }
    }
    
    public class GetJusticeUserListQueryHandler : IQueryHandler<GetJusticeUserListQuery, List<JusticeUser>>
    {
        private readonly BookingsDbContext _context;

        public GetJusticeUserListQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<JusticeUser>> Handle(GetJusticeUserListQuery query)
        {
            return await _context.JusticeUsers.Include(x => x.UserRole).ToListAsync();
        }
    }
}