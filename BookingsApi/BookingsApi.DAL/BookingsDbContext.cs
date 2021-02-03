using System;
using System.Linq;
using System.Reflection;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL
{
    public class BookingsDbContext : DbContext
    {
        public BookingsDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<VideoHearing> VideoHearings { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<CaseType> CaseTypes { get; set; }
        public DbSet<HearingVenue> Venues { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<LinkedParticipant> LinkedParticipant { get; set; }
        public DbSet<JobHistory> JobHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var applyGenericMethods =
                typeof(ModelBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                BindingFlags.FlattenHierarchy);
            var applyGenericApplyConfigurationMethods = applyGenericMethods.Where(m =>
                m.IsGenericMethod && m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));
            var applyGenericMethod = applyGenericApplyConfigurationMethods.FirstOrDefault(m =>
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
        }
        
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified))
            {
                var updatedDateProperty =
                    entry.Properties.AsQueryable().FirstOrDefault(x => x.Metadata.Name == "UpdatedDate");
                if (updatedDateProperty != null)
                {
                    updatedDateProperty.CurrentValue = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }
    }
}
