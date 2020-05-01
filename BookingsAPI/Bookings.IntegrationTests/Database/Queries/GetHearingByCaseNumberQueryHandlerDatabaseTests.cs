using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain.Participants;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetHearingByCaseNumberQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetHearingByCaseNumberQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingByCaseNumberQueryHandler(context);
        }

        [Test]
        public async Task Should_get_hearing_details_by_case_number()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: { seededHearing.Id }");
            var caseData = seededHearing.HearingCases.FirstOrDefault();
            TestContext.WriteLine($"New seeded video caseNumer: { caseData.Case.Number }");
            var hearing = await _handler.Handle(new GetHearingByCaseNumberQuery(caseData.Case.Number));

            hearing.Should().NotBeNull();

            var participants = hearing.GetParticipants();
            participants.Any().Should().BeTrue();
            var individuals = participants.Where(x => x.GetType() == typeof(Individual))
                .ToList();
            individuals.Should().NotBeNullOrEmpty();

            var representatives = participants.Where(x => x.GetType() == typeof(Representative));
            representatives.Should().NotBeNullOrEmpty();

            var judges = participants.Where(x => x.GetType() == typeof(Judge));
            judges.Should().NotBeNullOrEmpty();

            var persons = hearing.GetPersons();
            persons.Count.Should().Be(participants.Count);
            persons[0].Title.Should().NotBeEmpty();
            var cases = hearing.GetCases();
            hearing.GetCases().Any(x => x.IsLeadCase).Should().BeTrue();
            cases.Count.Should().Be(2);
            cases[0].Name.Should().NotBeEmpty();
            hearing.HearingRoomName.Should().NotBeEmpty();
            hearing.OtherInformation.Should().NotBeEmpty();
            hearing.CreatedBy.Should().NotBeNullOrEmpty();

            foreach (var individual in individuals)
            {
                var address = individual.Person.Address;
                address.Should().NotBeNull();
                address.HouseNumber.Should().NotBeNullOrEmpty();
                address.Street.Should().NotBeNullOrEmpty();
                address.Postcode.Should().NotBeNullOrEmpty();
            }
            hearing.AudioRecordingRequired.Should().BeTrue();
        }
    }
}