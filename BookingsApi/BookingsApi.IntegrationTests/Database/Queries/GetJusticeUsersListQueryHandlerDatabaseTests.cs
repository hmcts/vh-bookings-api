using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetJusticeUsersListQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetJusticeUserListQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetJusticeUserListQueryHandler(context);
        }

        [Test]
        public async Task Should_return_empty_list_when_no_users_are_found()
        {
            await Hooks.ClearAllJusticeUsersAsync();
            var query = new GetJusticeUserListQuery(null);
            var users = await _handler.Handle(query);

            users.Should().BeEmpty();
        }
        
        [Test]
        public async Task Should_return_users_list_not_null()
        {
            var query = new GetJusticeUserListQuery(null);
            var users = (await _handler.Handle(query));

            users.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_return_users_list_when_term_is_passed()
        {
            var term = "term";
            var query = new GetJusticeUserListQuery(term);
            var users = (await _handler.Handle(query));

            users.Should().NotBeNull();
        }

        [Test]
        public async Task Should_not_return_deleted_users_when_term_is_null()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            var justiceUser = await Hooks.SeedJusticeUser("getjusticeuserlist.deleted.cso1@email.com", "FirstName", "LastName");
            justiceUser = await db.JusticeUsers.FirstOrDefaultAsync(x => x.Id == justiceUser.Id);
            justiceUser.Delete();
            await db.SaveChangesAsync();
        
            var query = new GetJusticeUserListQuery(null);
            var users = await _handler.Handle(query);
        
            users.Should().NotContain(x => x.Id == justiceUser.Id);
        }
        
        [Test]
        public async Task Should_not_return_deleted_users_when_term_is_not_null()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            var justiceUser = await Hooks.SeedJusticeUser("getjusticeuserlist.deleted.cso2@email.com", "FirstName", "LastName");
            justiceUser = await db.JusticeUsers.FirstOrDefaultAsync(x => x.Id == justiceUser.Id);
            justiceUser.Delete();
            await db.SaveChangesAsync();
        
            var query = new GetJusticeUserListQuery("email.com");
            var users = await _handler.Handle(query);
        
            users.Should().NotContain(x => x.Id == justiceUser.Id);
        }
    }
}