using System;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookings.DAL.Mappings
{
    public class ChecklistAnswerMap : IEntityTypeConfiguration<ChecklistAnswer>
    {
        public void Configure(EntityTypeBuilder<ChecklistAnswer> builder)
        {
            builder.ToTable("ChecklistAnswer");

            builder.HasKey(x => x.Id);
            builder.Property(typeof(Guid), "ParticipantId");
            builder.Property(typeof(int), "ChecklistQuestionId");
            builder.Property(x => x.Answer);
            builder.Property(x => x.Notes);
            builder.Property(x => x.CreatedAt);
            builder.HasIndex("ParticipantId");
            builder.HasIndex("ParticipantId", "ChecklistQuestionId").IsUnique();

            // Because of MS SQL issues with "multiple cascade paths" we need to restrict one of the cascade paths
            // ensuring we only have one cascade to delete the answers. In practice this means, if you want to delete a question
            // make sure you delete all the answers to it first.
            builder.HasOne(x => x.Question)
                .WithMany()
                .HasForeignKey("ChecklistQuestionId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Participant>().WithMany(x => x.ChecklistAnswers)
                .HasForeignKey("ParticipantId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}