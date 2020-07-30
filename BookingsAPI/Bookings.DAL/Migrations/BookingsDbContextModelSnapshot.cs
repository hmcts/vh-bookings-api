﻿// <auto-generated />
using System;
using Bookings.DAL;
using Bookings.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bookings.DAL.Migrations
{
    [DbContext(typeof(BookingsDbContext))]
    partial class BookingsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Bookings.Domain.Case", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsLeadCase")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Case");
                });

            modelBuilder.Entity("Bookings.Domain.Hearing", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AudioRecordingRequired")
                        .HasColumnType("bit");

                    b.Property<string>("CancelReason")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("CaseTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ConfirmedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ConfirmedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("HearingMediumType")
                        .HasColumnName("HearingMediumId")
                        .HasColumnType("int");

                    b.Property<string>("HearingRoomName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HearingTypeId")
                        .HasColumnType("int");

                    b.Property<string>("HearingVenueName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OtherInformation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("QuestionnaireNotRequired")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ScheduledDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ScheduledDuration")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnName("HearingStatusId")
                        .HasColumnType("int");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CaseTypeId");

                    b.HasIndex("HearingTypeId");

                    b.HasIndex("HearingVenueName");

                    b.ToTable("Hearing");

                    b.HasDiscriminator<int>("HearingMediumType");
                });

            modelBuilder.Entity("Bookings.Domain.HearingCase", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CaseId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("HearingId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("HearingId");

                    b.HasIndex("CaseId", "HearingId")
                        .IsUnique();

                    b.ToTable("HearingCase");
                });

            modelBuilder.Entity("Bookings.Domain.HearingVenue", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Name");

                    b.ToTable("HearingVenue");
                });

            modelBuilder.Entity("Bookings.Domain.Organisation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Organisation");
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CaseRoleId")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("HearingId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("HearingRoleId")
                        .HasColumnType("int");

                    b.Property<Guid>("PersonId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CaseRoleId");

                    b.HasIndex("HearingId");

                    b.HasIndex("HearingRoleId");

                    b.HasIndex("PersonId", "HearingId")
                        .IsUnique();

                    b.ToTable("Participant");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Participant");
                });

            modelBuilder.Entity("Bookings.Domain.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContactEmail")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleNames")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("OrganisationId")
                        .HasColumnType("bigint");

                    b.Property<string>("TelephoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ContactEmail")
                        .IsUnique()
                        .HasFilter("[ContactEmail] IS NOT NULL");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasFilter("[Username] IS NOT NULL");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Bookings.Domain.Questionnaire", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId")
                        .IsUnique();

                    b.ToTable("Questionnaire");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.CaseRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseTypeId")
                        .HasColumnType("int");

                    b.Property<int>("Group")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CaseTypeId");

                    b.ToTable("CaseRole");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.CaseType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CaseType");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.HearingRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseRoleId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserRoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CaseRoleId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("HearingRole");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.HearingType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CaseTypeId");

                    b.ToTable("HearingType");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Bookings.Domain.SuitabilityAnswer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExtendedData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("QuestionnaireId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("QuestionnaireId");

                    b.ToTable("SuitabilityAnswer");
                });

            modelBuilder.Entity("Bookings.Domain.VideoHearing", b =>
                {
                    b.HasBaseType("Bookings.Domain.Hearing");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Individual", b =>
                {
                    b.HasBaseType("Bookings.Domain.Participants.Participant");

                    b.HasDiscriminator().HasValue("Individual");
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Judge", b =>
                {
                    b.HasBaseType("Bookings.Domain.Participants.Participant");

                    b.HasDiscriminator().HasValue("Judge");
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Representative", b =>
                {
                    b.HasBaseType("Bookings.Domain.Participants.Participant");

                    b.Property<string>("Reference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Representee")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Representative");
                });

            modelBuilder.Entity("Bookings.Domain.Hearing", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseType", "CaseType")
                        .WithMany()
                        .HasForeignKey("CaseTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bookings.Domain.RefData.HearingType", "HearingType")
                        .WithMany()
                        .HasForeignKey("HearingTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bookings.Domain.HearingVenue", "HearingVenue")
                        .WithMany()
                        .HasForeignKey("HearingVenueName");
                });

            modelBuilder.Entity("Bookings.Domain.HearingCase", b =>
                {
                    b.HasOne("Bookings.Domain.Case", "Case")
                        .WithMany("HearingCases")
                        .HasForeignKey("CaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bookings.Domain.Hearing", "Hearing")
                        .WithMany("HearingCases")
                        .HasForeignKey("HearingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Participant", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseRole", "CaseRole")
                        .WithMany()
                        .HasForeignKey("CaseRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bookings.Domain.Hearing", "Hearing")
                        .WithMany("Participants")
                        .HasForeignKey("HearingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bookings.Domain.RefData.HearingRole", "HearingRole")
                        .WithMany()
                        .HasForeignKey("HearingRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bookings.Domain.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bookings.Domain.Person", b =>
                {
                    b.HasOne("Bookings.Domain.Organisation", "Organisation")
                        .WithMany()
                        .HasForeignKey("OrganisationId");
                });

            modelBuilder.Entity("Bookings.Domain.Questionnaire", b =>
                {
                    b.HasOne("Bookings.Domain.Participants.Participant", "Participant")
                        .WithOne("Questionnaire")
                        .HasForeignKey("Bookings.Domain.Questionnaire", "ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bookings.Domain.RefData.CaseRole", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseType", null)
                        .WithMany("CaseRoles")
                        .HasForeignKey("CaseTypeId");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.HearingRole", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseRole", null)
                        .WithMany("HearingRoles")
                        .HasForeignKey("CaseRoleId");

                    b.HasOne("Bookings.Domain.RefData.UserRole", "UserRole")
                        .WithMany()
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bookings.Domain.RefData.HearingType", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseType", null)
                        .WithMany("HearingTypes")
                        .HasForeignKey("CaseTypeId");
                });

            modelBuilder.Entity("Bookings.Domain.SuitabilityAnswer", b =>
                {
                    b.HasOne("Bookings.Domain.Questionnaire", "Questionnaire")
                        .WithMany("SuitabilityAnswers")
                        .HasForeignKey("QuestionnaireId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
