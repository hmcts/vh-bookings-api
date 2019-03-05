using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Queries
{
    public class GetPersonByContactEmailQuery : IQuery
    {
        public string Email { get; set; }

        public GetPersonByContactEmailQuery(string email)
        {
            Email = email;
        }
    }
    
    public class GetPersonByContactEmailQueryHandler : IQueryHandler<GetPersonByContactEmailQuery, Person>
    {
        private readonly BookingsDbContext _context;

        public GetPersonByContactEmailQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<Person> Handle(GetPersonByContactEmailQuery query)
        {
            return await _context.Persons
                .Include(x => x.Address)
                .Include(x => x.Organisation)
                .SingleOrDefaultAsync(x => x.ContactEmail == query.Email);   
            
        }
    }
}