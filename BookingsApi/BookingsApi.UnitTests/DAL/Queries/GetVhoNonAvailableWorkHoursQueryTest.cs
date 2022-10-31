using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.DAL.Queries
{
    public class GetVhoNonAvailableWorkHoursQueryTest
    {
        [Test]
        public void Should_search_forJudiciary_person_by_term_lower_case()
        {
            var query = new GetVhoNonAvailableWorkHoursQuery("Tester");

            query.UserName.Should().Be("tester");
        }

        [Test]
        public void Should_create_handler()
        {
            var connectionstring = "Connection string";

            var optionsBuilder = new DbContextOptionsBuilder<BookingsDbContext>();
            optionsBuilder.UseSqlServer(connectionstring);


           var context = new BookingsDbContext(optionsBuilder.Options);

            var handler = new GetVhoNonAvailableWorkHoursQueryHandler(context);
            handler.Should().NotBeNull();
        }
    }
}


