using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UpdatePersonCommandTests : DatabaseTestsBase
    {
        private UpdatePersonCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UpdatePersonCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }

        [Test]
        public void should_throw_exception_when_peron_does_not_exist()
        {
            var command = new UpdatePersonCommand(Guid.NewGuid(), "New", "Me", "new.me@hmcts.net");
            Assert.ThrowsAsync<PersonNotFoundException>(() => _commandHandler.Handle(command));
        }

        [TestCase(null, "Me", "new.me@hmcts.net")]
        [TestCase("New", null, "new.me@hmcts.net")]
        [TestCase("New", "Me", null)]
        public void should_throw_exception_if_arguments_are_null(string firstName, string lastName, string username)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UpdatePersonCommand(Guid.NewGuid(), firstName, lastName, username));
        }

        [Test]
        public async Task should_update_person()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            var person = seededHearing.GetPersons().First();
            
            var command = new UpdatePersonCommand(person.Id, "New", "Me", "new.me@hmcts.net");

            await _commandHandler.Handle(command);

            var updatedHearing = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));

            var updatePerson = updatedHearing.GetPersons().First(x => x.Id == person.Id);
            updatePerson.FirstName.Should().Be(command.FirstName);
            updatePerson.LastName.Should().Be(command.LastName);
            updatePerson.Username.Should().Be(command.Username);
        }
    }
}