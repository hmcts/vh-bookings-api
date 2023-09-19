using Microsoft.EntityFrameworkCore.Storage;

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
            builder.ReplaceService<IRelationalCommandBuilderFactory, DynamicSqlRelationalCommandBuilderFactory>();
            var context = new RefDataContext(builder.Options);
            return context;
        }
    }

    [ExcludeFromCodeCoverage]
    public class DynamicSqlRelationalCommandBuilder : RelationalCommandBuilder
    {
        public DynamicSqlRelationalCommandBuilder(RelationalCommandBuilderDependencies dependencies) : base(dependencies)
        {
        }

        public override IRelationalCommand Build()
        {
            var commandText = base.Build().CommandText;
            commandText = "EXECUTE ('" + commandText.Replace("'", "''") + "')";
            return new RelationalCommand(Dependencies, commandText, Parameters);
        }
    }
    
    [ExcludeFromCodeCoverage]
    public class DynamicSqlRelationalCommandBuilderFactory : RelationalCommandBuilderFactory
    {
        public DynamicSqlRelationalCommandBuilderFactory(RelationalCommandBuilderDependencies dependencies) : base(dependencies)
        {
        }

        public override IRelationalCommandBuilder Create()
        {
            return new DynamicSqlRelationalCommandBuilder(Dependencies);
        }
    }
}