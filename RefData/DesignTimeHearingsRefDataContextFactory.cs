namespace RefData
{
    [ExcludeFromCodeCoverage]
    public class DesignTimeHearingsRefDataContextFactory : IDesignTimeDbContextFactory<RefDataContext>
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