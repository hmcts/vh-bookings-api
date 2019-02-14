using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HouseNumber = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    Postcode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Case",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsLeadCase = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Case", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistQuestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HearingVenue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HearingVenue", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Organisation",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Group = table.Column<int>(nullable: false),
                    CaseTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseRole_CaseType_CaseTypeId",
                        column: x => x.CaseTypeId,
                        principalTable: "CaseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HearingType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CaseTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HearingType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HearingType_CaseType_CaseTypeId",
                        column: x => x.CaseTypeId,
                        principalTable: "CaseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    MiddleNames = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    ContactEmail = table.Column<string>(nullable: true),
                    TelephoneNumber = table.Column<string>(nullable: true),
                    OrganisationId = table.Column<long>(nullable: true),
                    AddressId = table.Column<long>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Person_Organisation_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HearingRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    UserRoleId = table.Column<int>(nullable: false),
                    CaseRoleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HearingRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HearingRole_CaseRole_CaseRoleId",
                        column: x => x.CaseRoleId,
                        principalTable: "CaseRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HearingRole_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hearing",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HearingMediumId = table.Column<int>(nullable: false),
                    HearingVenueName = table.Column<string>(nullable: true),
                    CaseTypeId = table.Column<int>(nullable: false),
                    HearingTypeId = table.Column<int>(nullable: false),
                    ScheduledDateTime = table.Column<DateTime>(nullable: false),
                    ScheduledDuration = table.Column<int>(nullable: false),
                    HearingStatusId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hearing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hearing_CaseType_CaseTypeId",
                        column: x => x.CaseTypeId,
                        principalTable: "CaseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hearing_HearingType_HearingTypeId",
                        column: x => x.HearingTypeId,
                        principalTable: "HearingType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hearing_HearingVenue_HearingVenueName",
                        column: x => x.HearingVenueName,
                        principalTable: "HearingVenue",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HearingCase",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseId = table.Column<long>(nullable: false),
                    HearingId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HearingCase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HearingCase_Case_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Case",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HearingCase_Hearing_HearingId",
                        column: x => x.HearingId,
                        principalTable: "Hearing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participant",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    CaseRoleId = table.Column<int>(nullable: false),
                    HearingRoleId = table.Column<int>(nullable: false),
                    PersonId = table.Column<Guid>(nullable: false),
                    HearingId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    SolicitorsReference = table.Column<string>(nullable: true),
                    Representee = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participant_CaseRole_CaseRoleId",
                        column: x => x.CaseRoleId,
                        principalTable: "CaseRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participant_Hearing_HearingId",
                        column: x => x.HearingId,
                        principalTable: "Hearing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participant_HearingRole_HearingRoleId",
                        column: x => x.HearingRoleId,
                        principalTable: "HearingRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participant_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistAnswer",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Answer = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ChecklistQuestionId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswer_ChecklistQuestion_ChecklistQuestionId",
                        column: x => x.ChecklistQuestionId,
                        principalTable: "ChecklistQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChecklistAnswer_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseRole_CaseTypeId",
                table: "CaseRole",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswer_ChecklistQuestionId",
                table: "ChecklistAnswer",
                column: "ChecklistQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswer_ParticipantId",
                table: "ChecklistAnswer",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistAnswer_ParticipantId_ChecklistQuestionId",
                table: "ChecklistAnswer",
                columns: new[] { "ParticipantId", "ChecklistQuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistQuestion_RoleId_Key",
                table: "ChecklistQuestion",
                columns: new[] { "RoleId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hearing_CaseTypeId",
                table: "Hearing",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Hearing_HearingTypeId",
                table: "Hearing",
                column: "HearingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Hearing_HearingVenueName",
                table: "Hearing",
                column: "HearingVenueName");

            migrationBuilder.CreateIndex(
                name: "IX_HearingCase_HearingId",
                table: "HearingCase",
                column: "HearingId");

            migrationBuilder.CreateIndex(
                name: "IX_HearingCase_CaseId_HearingId",
                table: "HearingCase",
                columns: new[] { "CaseId", "HearingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HearingRole_CaseRoleId",
                table: "HearingRole",
                column: "CaseRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_HearingRole_UserRoleId",
                table: "HearingRole",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_HearingType_CaseTypeId",
                table: "HearingType",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_CaseRoleId",
                table: "Participant",
                column: "CaseRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_HearingId",
                table: "Participant",
                column: "HearingId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_HearingRoleId",
                table: "Participant",
                column: "HearingRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_PersonId_HearingId",
                table: "Participant",
                columns: new[] { "PersonId", "HearingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_AddressId",
                table: "Person",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_ContactEmail",
                table: "Person",
                column: "ContactEmail",
                unique: true,
                filter: "[ContactEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Person_OrganisationId",
                table: "Person",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_Username",
                table: "Person",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistAnswer");

            migrationBuilder.DropTable(
                name: "HearingCase");

            migrationBuilder.DropTable(
                name: "ChecklistQuestion");

            migrationBuilder.DropTable(
                name: "Participant");

            migrationBuilder.DropTable(
                name: "Case");

            migrationBuilder.DropTable(
                name: "Hearing");

            migrationBuilder.DropTable(
                name: "HearingRole");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "HearingType");

            migrationBuilder.DropTable(
                name: "HearingVenue");

            migrationBuilder.DropTable(
                name: "CaseRole");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Organisation");

            migrationBuilder.DropTable(
                name: "CaseType");
        }
    }
}
