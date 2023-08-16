using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.DAL.Commands
{
    public class DeleteNonWorkingHoursCommandTests
    {
        private BookingsDbContext _context;
        private DeleteNonWorkingHoursCommandHandler _handler;
        private JusticeUser _justiceUser;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            
            _justiceUser = new JusticeUser()
            {
                ContactEmail = "username@mail.com",
                Username = "username@mail.com",
                CreatedBy = "test@test.com",
                CreatedDate = DateTime.Now,
                FirstName = "firstName",
                Lastname = "lastName",
            };
            var userRoleCso = new UserRole((int)UserRoleId.Vho, "Video hearings officer");
            _justiceUser.JusticeUserRoles.Add(new JusticeUserRole(_justiceUser, userRoleCso));
            _justiceUser.AddOrUpdateNonAvailability(new DateTime(), new DateTime());

            _context.JusticeUsers.Add(_justiceUser);
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
            var slot = _justiceUser.VhoNonAvailability.First();
            var command = new DeleteNonWorkingHoursCommand(_justiceUser.Username, slot.Id);

            await _handler.Handle(command);

            var updatedJusticeUser = await _context.JusticeUsers.Include(x => x.VhoNonAvailability)
                .FirstAsync(x => x.Id == _justiceUser.Id);

            updatedJusticeUser.VhoNonAvailability.First(x=> x.Id == slot.Id).Deleted.Should().BeTrue();
        }
        
        [Test]
        public void should_not_delete_slot_for_not_valid_id()
        {
            var command = new DeleteNonWorkingHoursCommand(_justiceUser.Username, 99999999);
            
            Assert.ThrowsAsync<NonWorkingHoursNotFoundException>(async () => await _handler.Handle(command));
        }

    }

}