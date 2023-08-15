using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.DAL.Commands
{
    public class DeleteNonWorkingHoursCommandTests
    {
        private BookingsDbContext _context;
        private DeleteNonWorkingHoursCommandHandler _handler;
        
        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            _context.VhoNonAvailabilities.Add(
                new VhoNonAvailability()
                {
                    JusticeUser = new JusticeUser() { Username = "username@mail.com" },
                    StartTime = new DateTime(),
                    EndTime = new DateTime(),
                    JusticeUserId = Guid.NewGuid()
                }
            );
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
            _handler = new DeleteNonWorkingHoursCommandHandler(_context);
        }

        [Test]
        public async Task should_delete_slot_for_valid_id()
        {
            var slot = await _context.VhoNonAvailabilities.FirstOrDefaultAsync();
            var command = new DeleteNonWorkingHoursCommand(slot.Id);

            await _handler.Handle(command);
            var slotAfterDelete = await _context.VhoNonAvailabilities.FirstOrDefaultAsync();

            slotAfterDelete.Deleted.Should().BeTrue();
        }
        
        [Test]
        public async Task should_not_delete_slot_for_not_valid_id()
        {
            var slot = await _context.VhoNonAvailabilities.FirstOrDefaultAsync();
            var command = new DeleteNonWorkingHoursCommand(111);
            
            Assert.ThrowsAsync<NonWorkingHoursNotFoundException>(async () => await _handler.Handle(command));
        }

    }

}