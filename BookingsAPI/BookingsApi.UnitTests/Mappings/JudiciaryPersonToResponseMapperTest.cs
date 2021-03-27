using BookingsApi.Mappings;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Mappings
{
    public class JudiciaryPersonToResponseMapperTest
    {
        private readonly JudiciaryPersonToResponseMapper _mapper = new JudiciaryPersonToResponseMapper();

        [Test]
        public void Should_map_to_response()
        {
            var judiciaryPerson = new JudiciaryPersonBuilder().Build();

            var result = _mapper.MapJudiciaryPersonToResponse(judiciaryPerson);

            result.FirstName.Should().Be(judiciaryPerson.KnownAs);
            result.LastName.Should().Be(judiciaryPerson.Surname);
            result.Title.Should().Be(judiciaryPerson.Title);
            result.Username.Should().Be(judiciaryPerson.Email);
            result.Id.Should().Be(judiciaryPerson.Id);
        }
    }
}
