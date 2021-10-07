using BookingsApi.Contract.Configuration;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.DAL.Queries
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
        private readonly FeatureFlagConfiguration _featureFlagConfiguration;
        public static readonly List<string> excludedRoles = new List<string>() { "Judge", "JudicialOfficeHolder", "StaffMember" };

        public GetPersonBySearchTermQueryHandler(BookingsDbContext context, IOptions<FeatureFlagConfiguration> featureFlagConfigurationOptions)
        {
            _context = context;
            _featureFlagConfiguration = featureFlagConfigurationOptions.Value;
        }

        public async Task<List<Person>> Handle(GetPersonBySearchTermQuery query)
        {
            List<Person> results;
            if (_featureFlagConfiguration.EJudFeature)
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
