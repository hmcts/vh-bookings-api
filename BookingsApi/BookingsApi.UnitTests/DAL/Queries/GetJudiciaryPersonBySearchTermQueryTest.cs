using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.UnitTests.DAL.Queries
{
    public class GetJudiciaryPersonBySearchTermQueryTest
    {
        [Test]
        public void Should_search_forJudiciary_person_by_term_lower_case()
        {
            var query = new GetJudiciaryPersonBySearchTermQuery("Tester");

            query.Term.Should().Be("tester");
        }
    }
}


