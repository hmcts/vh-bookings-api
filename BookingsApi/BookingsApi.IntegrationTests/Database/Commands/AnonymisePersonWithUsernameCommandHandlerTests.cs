using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.DAL.Queries.V1;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AnonymisePersonWithUsernameCommandHandlerTests : DatabaseTestsBase
    {
        private AnonymisePersonWithUsernameCommandHandler _commandHandler;
        private GetAnonymisationDataQueryHandler _getAnonymisationDataQueryHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AnonymisePersonWithUsernameCommandHandler(context);
            _getAnonymisationDataQueryHandler = new GetAnonymisationDataQueryHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public async Task Anonymise_Person_Entries_For_Hearings_Older_Than_3_Months()
        {
            var seededHearing = await Hooks.SeedPastHearings(DateTime.Today.AddMonths(-3));
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var anonymisationDataQuery =
                await _getAnonymisationDataQueryHandler.Handle(new GetAnonymisationDataQuery());

            var usernamesToAnonymise = anonymisationDataQuery.Usernames;

            foreach (var username in usernamesToAnonymise)
            {
                await _commandHandler.Handle(new AnonymisePersonWithUsernameCommand { Username = username });
            }

            //fetching updated hearing
            var returnedVideoHearingAfterFirstAnonymisationRequest =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            returnedVideoHearingAfterFirstAnonymisationRequest.Should().NotBeNull();

            foreach (var person in seededHearing.GetPersons())
            {
                var updatedPerson = returnedVideoHearingAfterFirstAnonymisationRequest.GetPersons().FirstOrDefault(p => p.Id == person.Id);
                updatedPerson.FirstName.Should().NotBe(person.FirstName);
                updatedPerson.LastName.Should().NotBe(person.LastName);
                updatedPerson.MiddleNames.Should().NotBe(person.MiddleNames);
                updatedPerson.Username.Should().NotBe(person.Username);
                updatedPerson.ContactEmail.Should().NotBe(person.ContactEmail);
                updatedPerson.TelephoneNumber.Should().NotBe(person.TelephoneNumber);
            }

            //after anonymisation is complete, usernames to anonymise should be empty
            var returnedVideoHearingAfterSecondAnonymisationRequest =
                (await _getAnonymisationDataQueryHandler.Handle(new GetAnonymisationDataQuery())).Usernames;

            returnedVideoHearingAfterSecondAnonymisationRequest.Count.Should().Be(0);
        }
    }
}