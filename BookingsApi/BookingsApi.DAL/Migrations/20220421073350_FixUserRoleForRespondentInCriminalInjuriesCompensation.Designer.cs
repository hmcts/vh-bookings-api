﻿// <auto-generated />
using System;
using BookingsApi.DAL;
using BookingsApi.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookingsApi.DAL.Migrations
{
    [DbContext(typeof(BookingsDbContext))]
    [Migration("20220421073350_FixUserRoleForRespondentInCriminalInjuriesCompensation")]
    partial class FixUserRoleForRespondentInCriminalInjuriesCompensation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BookingsApi.Domain.Case", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsLeadCase")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Case");
                });

            modelBuilder.Entity("BookingsApi.Domain.Endpoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DefenceAdvocateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("HearingId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Pin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sip")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DefenceAdvocateId");

                    b.HasIndex("HearingId");

                    b.HasIndex("Sip")
                        .IsUnique()
                        .HasFilter("[Sip] IS NOT NULL");

                    b.ToTable("Endpoint");
                });

            modelBuilder.Entity("BookingsApi.Domain.Hearing", b =>
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

                    b.Property<Guid?>("SourceId")
                        .HasColumnType("uniqueidentifier");

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

            modelBuilder.Entity("BookingsApi.Domain.HearingCase", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CaseId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("HearingId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("HearingId");

                    b.HasIndex("CaseId", "HearingId")
                        .IsUnique();

                    b.ToTable("HearingCase");
                });

            modelBuilder.Entity("BookingsApi.Domain.HearingVenue", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Name");

                    b.ToTable("HearingVenue");
                });

            modelBuilder.Entity("BookingsApi.Domain.JobHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastRunDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("JobHistory");
                });

            modelBuilder.Entity("BookingsApi.Domain.JudiciaryPerson", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ExternalRefId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Fullname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("HasLeft")
                        .HasColumnType("bit");

                    b.Property<string>("KnownAs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PersonalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostNominals")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ExternalRefId")
                        .IsUnique();

                    b.ToTable("JudiciaryPerson");
                });

            modelBuilder.Entity("BookingsApi.Domain.LinkedParticipant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("LinkedId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("LinkedId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("LinkedParticipant");
                });

            modelBuilder.Entity("BookingsApi.Domain.Organisation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Organisation");
                });

            modelBuilder.Entity("BookingsApi.Domain.Participants.Participant", b =>
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

            modelBuilder.Entity("BookingsApi.Domain.Person", b =>
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

            modelBuilder.Entity("BookingsApi.Domain.Questionnaire", b =>
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

            modelBuilder.Entity("BookingsApi.Domain.RefData.CaseRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Group")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CaseTypeId");

                    b.ToTable("CaseRole");
                });

            modelBuilder.Entity("BookingsApi.Domain.RefData.CaseType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("CaseType");
                });

            modelBuilder.Entity("BookingsApi.Domain.RefData.HearingRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseRoleId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Live")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserRoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CaseRoleId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("HearingRole");
                });

            modelBuilder.Entity("BookingsApi.Domain.RefData.HearingType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Live")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CaseTypeId");

                    b.ToTable("HearingType");
                });

            modelBuilder.Entity("BookingsApi.Domain.RefData.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("BookingsApi.Domain.SuitabilityAnswer", b =>
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

            modelBuilder.Entity("BookingsApi.Domain.VideoHearing", b =>
                {
                    b.HasBaseType("BookingsApi.Domain.Hearing");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("BookingsApi.Domain.Participants.Individual", b =>
                {
                    b.HasBaseType("BookingsApi.Domain.Participants.Participant");

                    b.HasDiscriminator().HasValue("Individual");
                });

            modelBuilder.Entity("BookingsApi.Domain.Participants.Judge", b =>
                {
                    b.HasBaseType("BookingsApi.Domain.Participants.Participant");

                    b.HasDiscriminator().HasValue("Judge");
                });

            modelBuilder.Entity("BookingsApi.Domain.Participants.JudicialOfficeHolder", b =>
                {
                    b.HasBaseType("BookingsApi.Domain.Participants.Participant");

                    b.HasDiscriminator().HasValue("JudicialOfficeHolder");
                });

            modelBuilder.Entity("BookingsApi.Domain.Participants.Representative", b =>
                {
                    b.HasBaseType("BookingsApi.Domain.Participants.Participant");

                    b.Property<string>("Representee")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Representative");
                });

            modelBuilder.Entity("BookingsApi.Domain.Participants.StaffMember", b =>
                {
                    b.HasBaseType("BookingsApi.Domain.Participants.Participant");

                    b.HasDiscriminator().HasValue("StaffMember");
                });

            modelBuilder.Entity("BookingsApi.Domain.Endpoint", b =>
                {
                    b.HasOne("BookingsApi.Domain.Participants.Participant", "DefenceAdvocate")
                        .WithMany()
                        .HasForeignKey("DefenceAdvocateId");

                    b.HasOne("BookingsApi.Domain.Hearing", "Hearing")
                        .WithMany("Endpoints")
                        .HasForeignKey("HearingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookingsApi.Domain.Hearing", b =>
                {
                    b.HasOne("BookingsApi.Domain.RefData.CaseType", "CaseType")
                        .WithMany()
                        .HasForeignKey("CaseTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingsApi.Domain.RefData.HearingType", "HearingType")
                        .WithMany()
                        .HasForeignKey("HearingTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingsApi.Domain.HearingVenue", "HearingVenue")
                        .WithMany()
                        .HasForeignKey("HearingVenueName");
                });

            modelBuilder.Entity("BookingsApi.Domain.HearingCase", b =>
                {
                    b.HasOne("BookingsApi.Domain.Case", "Case")
                        .WithMany("HearingCases")
                        .HasForeignKey("CaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingsApi.Domain.Hearing", "Hearing")
                        .WithMany("HearingCases")
                        .HasForeignKey("HearingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookingsApi.Domain.LinkedParticipant", b =>
                {
                    b.HasOne("BookingsApi.Domain.Participants.Participant", "Linked")
                        .WithMany()
                        .HasForeignKey("LinkedId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("BookingsApi.Domain.Participants.Participant", "Participant")
                        .WithMany("LinkedParticipants")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookingsApi.Domain.Participants.Participant", b =>
                {
                    b.HasOne("BookingsApi.Domain.RefData.CaseRole", "CaseRole")
                        .WithMany()
                        .HasForeignKey("CaseRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingsApi.Domain.Hearing", "Hearing")
                        .WithMany("Participants")
                        .HasForeignKey("HearingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingsApi.Domain.RefData.HearingRole", "HearingRole")
                        .WithMany()
                        .HasForeignKey("HearingRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingsApi.Domain.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookingsApi.Domain.Person", b =>
                {
                    b.HasOne("BookingsApi.Domain.Organisation", "Organisation")
                        .WithMany()
                        .HasForeignKey("OrganisationId");
                });

            modelBuilder.Entity("BookingsApi.Domain.Questionnaire", b =>
                {
                    b.HasOne("BookingsApi.Domain.Participants.Participant", "Participant")
                        .WithOne("Questionnaire")
                        .HasForeignKey("BookingsApi.Domain.Questionnaire", "ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookingsApi.Domain.RefData.CaseRole", b =>
                {
                    b.HasOne("BookingsApi.Domain.RefData.CaseType", null)
                        .WithMany("CaseRoles")
                        .HasForeignKey("CaseTypeId");
                });

            modelBuilder.Entity("BookingsApi.Domain.RefData.HearingRole", b =>
                {
                    b.HasOne("BookingsApi.Domain.RefData.CaseRole", null)
                        .WithMany("HearingRoles")
                        .HasForeignKey("CaseRoleId");

                    b.HasOne("BookingsApi.Domain.RefData.UserRole", "UserRole")
                        .WithMany()
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookingsApi.Domain.RefData.HearingType", b =>
                {
                    b.HasOne("BookingsApi.Domain.RefData.CaseType", null)
                        .WithMany("HearingTypes")
                        .HasForeignKey("CaseTypeId");
                });

            modelBuilder.Entity("BookingsApi.Domain.SuitabilityAnswer", b =>
                {
                    b.HasOne("BookingsApi.Domain.Questionnaire", "Questionnaire")
                        .WithMany("SuitabilityAnswers")
                        .HasForeignKey("QuestionnaireId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
