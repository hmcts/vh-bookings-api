using System.Text.RegularExpressions;
using BookingsApi.Extensions;
using Testing.Common.Builders.Api.V2;

namespace BookingsApi.UnitTests.Extensions;

public partial class ContractExtensionTests
{
    [GeneratedRegex(@"^\S(.*\S)?$")]
    private static partial Regex NameRegex();
    
    [TestCase("FirstNameWithSpaces ", "LastNameWithSpaces ")]
    [TestCase(" FirstNameWithSpaces",  " LastNameWithSpaces")]
    [TestCase(" FirstNameWithSpaces ", " LastNameWithSpaces ")]
    [TestCase(" FirstName WithSpaces "," LastName WithSpaces ")]
    public void should_sanitize_booking_request_v2(string firstName, string lastName)
    {
        // arrange
        var request = new SimpleBookNewHearingRequestV2("foo2", DateTime.UtcNow, "RandomPersonCode").Build();   
        request.Participants[0].FirstName = firstName;
        request.Participants[0].LastName = lastName;


        // act
        request.SanitizeRequest();

        // assert
        // regex for no white spaces at the start or end of the string 
        var regex = NameRegex();
        request.Participants[0].FirstName.Should().MatchRegex(regex);
        request.Participants[0].LastName.Should().MatchRegex(regex);
    }
}