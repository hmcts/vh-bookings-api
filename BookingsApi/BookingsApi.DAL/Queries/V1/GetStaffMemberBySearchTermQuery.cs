using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetStaffMemberBySearchTermQuery : IQuery
    {
        public string Term { get; }

        public GetStaffMemberBySearchTermQuery(string term)
        {
            Term = term.ToLowerInvariant();
        }
    }

    public class GetStaffMemberBySearchTermQueryHandler : IQueryHandler<GetStaffMemberBySearchTermQuery, List<Person>>
    {
        private readonly BookingsDbContext _context;

        public GetStaffMemberBySearchTermQueryHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Person>> Handle(GetStaffMemberBySearchTermQuery query)
        {
            var includeRoles = new List<string>() { "StaffMember" };

            var results = await (from person in _context.Persons
             join participant in _context.Participants on person.Id equals participant.PersonId
             where includeRoles.Contains(participant.Discriminator) 
             && person.ContactEmail.ToLower().Contains(query.Term.ToLower())
             select person).Distinct().Include(x => x.Organisation).ToListAsync();

            return results;
        }
    }
}
