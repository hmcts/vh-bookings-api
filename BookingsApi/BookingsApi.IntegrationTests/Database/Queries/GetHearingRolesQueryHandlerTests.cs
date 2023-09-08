using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries;

public class GetHearingRolesQueryHandlerTests : DatabaseTestsBase
{
    private GetHearingRolesQueryHandler _queryHandler;

    [SetUp]
    public void Setup()
    {
        var context = new BookingsDbContext(BookingsDbContextOptions);
        _queryHandler = new GetHearingRolesQueryHandler(context);
    }

    [Test]
    public async Task Should_get_all_hearing_roles()
    {
        // arrange & act
        var hearingRoles = await _queryHandler.Handle(new GetHearingRolesQuery());

        // assert
        var expected = new List<string>()
        {
            "Applicant",
            "Appellant",
            "Barrister",
            "Claimant",
            "Defendant",
            "Expert",
            "Welfare Representative",
            "Intermediaries",
            "Interpreter",
            "Joint Party",
            "Legal Representative",
            "Litigation Friend",
            "Observer",
            "Other Party",
            "Police",
            "Respondent",
            "Support",
            "Panel Member",
            "Witness",
            "Judge",
            "Staff Member"
        };
        
        hearingRoles.Should().NotBeEmpty();
        hearingRoles.Select(x=> x.Name).Should().BeEquivalentTo(expected);
        
    }
}