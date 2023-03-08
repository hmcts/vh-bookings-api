using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.UnitTests.DAL.Commands
{
    public class UpdatePersonUsernameCommandTests
    {
        private BookingsDbContext _context;
        private UpdatePersonUsernameCommandHandler _handler;
        
        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            _context.Persons.Add(new Person("Mr", "test", "one", "test.one@one.com", ""));
            _context.SaveChangesAsync();

        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [SetUp]
        public void Setup()
        {
            _handler = new UpdatePersonUsernameCommandHandler(_context);
        }

        [Test]
        public async Task should_update_person_username()
        {
            var person = await _context.Persons.FirstOrDefaultAsync();
            var command = new UpdatePersonUsernameCommand(person.Id, "justone@one.com");

            await _handler.Handle(command);
            var person1 = await _context.Persons.FirstOrDefaultAsync();

            person1.Username.Should().Be("justone@one.com");
        }

        [Test]
        public void should_throw_exception_when_person_not_found()
        {
            var command = new UpdatePersonUsernameCommand(Guid.NewGuid(), "justone@one.com");
            Assert.ThrowsAsync<PersonNotFoundException>(async () => await _handler.Handle(command));
        }
    }

}