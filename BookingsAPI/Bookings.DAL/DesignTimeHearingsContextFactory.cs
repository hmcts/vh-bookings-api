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
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F")
                .Build();
            var builder = new DbContextOptionsBuilder<BookingsDbContext>();
            builder.UseSqlServer(config.GetConnectionString("VhBookings"));
            var context = new BookingsDbContext(builder.Options);
            return context;
        }
    }

}