using System.Reflection;
using System.Threading;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL
{
    public class BookingsDbContext : DbContext
    {
        public BookingsDbContext(DbContextOptions options) : base(options){}
        public DbSet<Case> Cases { get; set; }
        public DbSet<VideoHearing> VideoHearings { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<CaseType> CaseTypes { get; set; }
        public DbSet<Domain.DayOfWeek> DaysOfWeek { get; set; }
        public DbSet<HearingVenue> Venues { get; set; }
        public DbSet<HearingRole> HearingRoles { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<JobHistory> JobHistory { get; set; }
        public DbSet<JudiciaryPerson> JudiciaryPersons { get; set; }
        public DbSet<JusticeUser> JusticeUsers { get; set; }
        public DbSet<JudiciaryPersonStaging> JudiciaryPersonsStaging { get; set; }
        public DbSet<Jurisdiction> Jurisdiction { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<JusticeUserRole> JusticeUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var applyGenericMethods =
                typeof(ModelBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                BindingFlags.FlattenHierarchy);
            var applyGenericApplyConfigurationMethods = applyGenericMethods.Where(m =>
                m.IsGenericMethod && m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));
            var applyGenericMethod = applyGenericApplyConfigurationMethods.First(m =>
                m.GetParameters().FirstOrDefault()?.ParameterType.Name == "IEntityTypeConfiguration`1");

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(c => c.IsClass && !c.IsAbstract && !c.ContainsGenericParameters))
            {
                foreach (var iface in type.GetInterfaces())
                {
                    if (iface.IsConstructedGenericType &&
                        iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {

                        var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(modelBuilder, new object[] {Activator.CreateInstance(type)});
                        break;
                    }
                }
            }

            modelBuilder.Entity<JusticeUser>().HasQueryFilter(u => !u.Deleted);
            modelBuilder.Entity<VhoWorkHours>().HasQueryFilter(wh => !wh.Deleted);
        }
        
        public override int SaveChanges()
        {
            SetUpdatedDateValue();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetUpdatedDateValue();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetUpdatedDateValue()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified))
            {
                var updatedDateProperty = entry.Properties.AsQueryable().FirstOrDefault(x => x.Metadata.Name == "UpdatedDate");
                if (updatedDateProperty != null)
                {
                    updatedDateProperty.CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}
