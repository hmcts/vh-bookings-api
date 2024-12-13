using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.DAL
{
    [ExcludeFromCodeCoverage]
    public class DesignTimeHearingsContextFactory : IDesignTimeDbContextFactory<BookingsDbContext>
    {
        public BookingsDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();
            Console.WriteLine(config.GetConnectionString("VhBookings"));
            var builder = new DbContextOptionsBuilder<BookingsDbContext>();
            builder.UseSqlServer(config.GetConnectionString("VhBookings"));
            var context = new BookingsDbContext(builder.Options);
            return context;
        }
    }

}