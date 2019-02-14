using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Bookings.DAL
{
    public class DesignTimeHearingsContextFactory : IDesignTimeDbContextFactory<BookingsDbContext>
    {
        public BookingsDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("settings-config.json")
                .AddUserSecrets("d76b6eb8-f1a2-4a51-9b8f-21e1b6b81e4f")
                .Build();
            var builder = new DbContextOptionsBuilder<BookingsDbContext>();
            builder.UseSqlServer(config.GetConnectionString("VhListings"));
            var context = new BookingsDbContext(builder.Options);
            return context;
        }
    }

}