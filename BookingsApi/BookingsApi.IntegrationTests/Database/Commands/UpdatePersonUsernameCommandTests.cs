using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands.V1;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries.V1;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdatePersonUsernameCommandTests : DatabaseTestsBase
    {
        private UpdatePersonUsernameCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdatePersonUsernameCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public void should_throw_exception_when_peron_does_not_exist()
        {
            var command = new UpdatePersonUsernameCommand(Guid.NewGuid(),  "new.me@hmcts.net");
            Assert.ThrowsAsync<PersonNotFoundException>(() => _commandHandler.Handle(command));
        }

        [TestCase(null)]
        public void should_throw_exception_if_arguments_are_null(string username)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UpdatePersonUsernameCommand(Guid.NewGuid(), username));
        }

        [Test]
        public async Task should_update_person()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var person = seededHearing.GetPersons().First();
            
            var command = new UpdatePersonUsernameCommand(person.Id, "new.me@hmcts.net");

            await _commandHandler.Handle(command);

            var updatedHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            var updatePerson = updatedHearing.GetPersons().First(x => x.Id == person.Id);
            updatePerson.Username.Should().Be(command.Username);
        }
    }
}