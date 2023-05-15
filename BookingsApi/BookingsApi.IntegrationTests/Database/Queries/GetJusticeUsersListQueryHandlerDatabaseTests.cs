using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
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
        public async Task Should_return_deleted_users_when_term_is_not_null()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            var justiceUser = await Hooks.SeedJusticeUser("getjusticeuserlist.deleted.cso2@email.com", "FirstName", "LastName");
            justiceUser = await db.JusticeUsers.FirstOrDefaultAsync(x => x.Id == justiceUser.Id);
            justiceUser.Delete();
            await db.SaveChangesAsync();
        
            var query = new GetJusticeUserListQuery("email.com", true);
            var users = await _handler.Handle(query);
        
            users.Should().Contain(x => x.Id == justiceUser.Id);
        }
        
        [Test]
        public async Task Should_return_deleted_users()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            var justiceUser = await Hooks.SeedJusticeUser("getjusticeuserlist.deleted.cso2@email.com", "FirstName", "LastName");
            justiceUser = await db.JusticeUsers.FirstOrDefaultAsync(x => x.Id == justiceUser.Id);
            justiceUser.Delete();
            await db.SaveChangesAsync();
        
            var query = new GetJusticeUserListQuery("email.com", true);
            var users = await _handler.Handle(query);
        
            users.Should().Contain(x => x.Id == justiceUser.Id);
        }

        [Test]
        public async Task Should_sort_correctly_when_no_term_passed()
        {
            var user1 = await Hooks.SeedJusticeUser("getjusticeuserlist.cso1@email.com", "B", "B");
            var user2 = await Hooks.SeedJusticeUser("getjusticeuserlist.cso2@email.com", "D", "C");
            var user3 = await Hooks.SeedJusticeUser("getjusticeuserlist.cso3@email.com", "A", "B");
            var user4 = await Hooks.SeedJusticeUser("getjusticeuserlist.cso4@email.com", "C", "A");
            
            var query = new GetJusticeUserListQuery(null);
            var users = await _handler.Handle(query);

            users.Count.Should().Be(4);
            users.Select(u => u.Id).Should().Equal(new List<System.Guid> { user4.Id, user3.Id, user1.Id, user2.Id });
        }

        [Test]
        public async Task Should_sort_correctly_when_term_passed()
        {
            var user1 = await Hooks.SeedJusticeUser("getjusticeuserlist.cso1@email.com", "B", "B");
            var user2 = await Hooks.SeedJusticeUser("getjusticeuserlist.cso2@email.com", "D", "C");
            var user3 = await Hooks.SeedJusticeUser("getjusticeuserlist.cso3@email.com", "A", "B");
            var user4 = await Hooks.SeedJusticeUser("getjusticeuserlist.cso4@email.com", "C", "A");
            await Hooks.SeedJusticeUser("getjusticeuserlist.cso5@test.com", "E", "A");
            
            var query = new GetJusticeUserListQuery("email.com");
            var users = await _handler.Handle(query);

            users.Count.Should().Be(4);
            users.Select(u => u.Id).Should().Equal(new List<System.Guid> { user4.Id, user3.Id, user1.Id, user2.Id });
        }
    }
}