using BookingsApi.Mappings.V1;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class JudiciaryPersonToResponseMapperTests
    {
        [Test]
        public void Should_map_judiciary_person()
        {
            // Arrange
            var judiciaryPerson = new JudiciaryPersonBuilder("PersonalCode", isGeneric: true).Build();
            var mapper = new JudiciaryPersonToResponseMapper();
            
            // Act
            var response = mapper.MapJudiciaryPersonToResponse(judiciaryPerson);
            
            // Assert
            response.Title.Should().Be(judiciaryPerson.Title);
            response.FirstName.Should().Be(judiciaryPerson.KnownAs);
            response.LastName.Should().Be(judiciaryPerson.Surname);
            response.FullName.Should().Be(judiciaryPerson.Fullname);
            response.PersonalCode.Should().Be(judiciaryPerson.PersonalCode);
            response.Email.Should().Be(judiciaryPerson.Email.ToLower());
            response.IsGeneric.Should().Be(judiciaryPerson.IsGeneric);
        }
    }
}
