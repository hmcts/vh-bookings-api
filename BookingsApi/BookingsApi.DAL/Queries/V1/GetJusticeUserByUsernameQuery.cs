using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetJusticeUserByUsernameQuery : IQuery
    {
        public string Username { get; set; }

        public GetJusticeUserByUsernameQuery(string username)
        {
            Username = username;
        }
    }
    
    public class GetJusticeUserByUsernameQueryHandler : IQueryHandler<GetJusticeUserByUsernameQuery, JusticeUser>
    {
        private readonly BookingsDbContext _context;

        public GetJusticeUserByUsernameQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<JusticeUser> Handle(GetJusticeUserByUsernameQuery query)
        {
            return await _context.JusticeUsers
                .Include(x => x.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
                .SingleOrDefaultAsync(x => x.Username == query.Username);
        }
    }
}