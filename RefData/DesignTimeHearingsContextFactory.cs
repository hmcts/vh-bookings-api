using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RefData
{
    [ExcludeFromCodeCoverage]
    public class DesignTimeHearingsContextFactory : IDesignTimeDbContextFactory<RefDataContext>
    {
        public RefDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets("D76B6EB8-F1A2-4A51-9B8F-21E1B6B81E4F")
                .AddEnvironmentVariables()
                .Build();
            var builder = new DbContextOptionsBuilder<RefDataContext>();
            builder.UseSqlServer(config.GetConnectionString("VhBookings"));
            var context = new RefDataContext(builder.Options);
            return context;
        }
    }

}