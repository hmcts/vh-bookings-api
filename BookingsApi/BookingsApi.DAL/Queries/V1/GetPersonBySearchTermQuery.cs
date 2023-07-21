using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Queries.V1
{
    public class GetPersonBySearchTermQuery : IQuery
    {
        public string Term { get; }

        public GetPersonBySearchTermQuery(string term)
        {
            Term = term.ToLowerInvariant();
        }
    }

    public class GetPersonBySearchTermQueryHandler : IQueryHandler<GetPersonBySearchTermQuery, List<Person>>
    {
        private readonly BookingsDbContext _context;
        private readonly IFeatureToggles _featureToggles;
        public static readonly List<string> excludedRoles = new List<string>() { "Judge", "JudicialOfficeHolder", "StaffMember" };

        public GetPersonBySearchTermQueryHandler(BookingsDbContext context, IFeatureToggles featureToggles)
        {
            _context = context;
            _featureToggles = featureToggles;
        }

        public async Task<List<Person>> Handle(GetPersonBySearchTermQuery query)
        {
            List<Person> results;
            if (_featureToggles.EJudFeature())
            {
                results = await (from person in _context.Persons
                                 join participant in _context.Participants on person.Id equals participant.PersonId
                                 where !excludedRoles.Contains(participant.Discriminator)
                                 && person.ContactEmail.ToLower().Contains(query.Term.ToLower())
                                 select person).Distinct().Include(x => x.Organisation).ToListAsync();

            }
            else
            {
                results = await _context.Persons
                 .Include(x => x.Organisation)
                 .Where(x => x.ContactEmail.ToLower().Contains(query.Term.ToLower()))
                 .ToListAsync();
            }

            return results;
        }
    }
}
