using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries.Core;
using Microsoft.EntityFrameworkCore;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Configuration;

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
        private readonly IFeatureFlagService _featureFlagService;
        public static readonly List<string> excludedRoles = new List<string>() { "Judge", "JudicialOfficeHolder", "StaffMember" };

        public GetPersonBySearchTermQueryHandler(BookingsDbContext context, IFeatureFlagService featureFlagService)
        {
            _context = context;
            _featureFlagService = featureFlagService;
        }

        public async Task<List<Person>> Handle(GetPersonBySearchTermQuery query)
        {
            List<Person> results;
            var ejudFeatureFlag = _featureFlagService.GetFeatureFlag(nameof(FeatureFlags.EJudFeature));
            if (ejudFeatureFlag)
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
