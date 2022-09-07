using System;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries
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
            return await _context.JusticeUsers.Include(x => x.UserRole).SingleOrDefaultAsync(x => x.Username == query.Username);
        }
    }
}