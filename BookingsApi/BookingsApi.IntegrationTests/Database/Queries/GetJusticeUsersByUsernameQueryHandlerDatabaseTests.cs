using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetJusticeUsersByUsernameQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetJusticeUserByUsernameQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetJusticeUserByUsernameQueryHandler(context);
        }

        [Test]
        public async Task Should_return_null_when_no_user_is_found()
        {
            var query = new GetJusticeUserByUsernameQuery("doesnt.existatall@hmcts.net");
            var participants = await _handler.Handle(query);

            participants.Should().BeNull();
        }
        
        [Test]
        public async Task Should_return_participant_that_exists()
        {
            var justiceUser = await Hooks.SeedJusticeUser("team.lead@hearings.reform.hmcts.net", "firstName", "secondname", true);
            TestContext.WriteLine($"New seeded justice user id: {justiceUser.Id}");

            var query = new GetJusticeUserByUsernameQuery(justiceUser.Username);
            var user = (await _handler.Handle(query));

            user.Should().NotBeNull();
            user.Id.Should().Be(justiceUser.Id);
            user.Username.Should().Be(justiceUser.Username);
            user.UserRoleId.Should().Be(justiceUser.UserRoleId);
        }

        [Test]
        public async Task Should_return_null_when_user_is_deleted()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions); 
            
            var justiceUser = await Hooks.SeedJusticeUser("deleted.cso@hearings.reform.hmcts.net", "firstName", "secondname", true);
            justiceUser = await db.JusticeUsers.FirstOrDefaultAsync(x => x.Id == justiceUser.Id);
            justiceUser.Delete();
            await db.SaveChangesAsync();
            
            var query = new GetJusticeUserByUsernameQuery(justiceUser.Username);
            var user = (await _handler.Handle(query));

            user.Should().BeNull();
        }
    }
}