﻿namespace BookingsApi.DAL.Mappings
{
    public class EndpointMap : IEntityTypeConfiguration<Endpoint>
    {
        public void Configure(EntityTypeBuilder<Endpoint> builder)
        {
            builder.ToTable("Endpoint");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.DisplayName).IsRequired();
            builder.HasIndex(x => x.Sip).IsUnique();
            builder.Property(x => x.Pin).IsRequired();
            builder.HasOne<Hearing>("Hearing").WithMany("Endpoints").HasForeignKey(x => x.HearingId);
            builder.HasMany(u => u.EndpointLinkedParticipants)
                .WithOne(x => x.Endpoint)
                .HasForeignKey(x => x.EndpointId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}