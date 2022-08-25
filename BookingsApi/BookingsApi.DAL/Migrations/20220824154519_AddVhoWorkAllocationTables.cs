using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddVhoWorkAllocationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DayOfWeek",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayOfWeek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JusticeUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    ContactEmail = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    UserRole = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JusticeUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Allocation",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    HearingId = table.Column<Guid>(nullable: false),
                    JusticeUserId = table.Column<Guid>(nullable: false),
                    EffortSpent = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Allocation_Hearing_HearingId",
                        column: x => x.HearingId,
                        principalTable: "Hearing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allocation_JusticeUser_JusticeUserId",
                        column: x => x.JusticeUserId,
                        principalTable: "JusticeUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VhoNonAvailability",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    JusticeUserId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<TimeSpan>(nullable: false),
                    EndTime = table.Column<TimeSpan>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VhoNonAvailability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VhoNonAvailability_JusticeUser_JusticeUserId",
                        column: x => x.JusticeUserId,
                        principalTable: "JusticeUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VhoWorkHours",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    JusticeUserId = table.Column<Guid>(nullable: false),
                    DayOfWeekId = table.Column<int>(nullable: false),
                    StartTime = table.Column<TimeSpan>(nullable: false),
                    EndTime = table.Column<TimeSpan>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VhoWorkHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VhoWorkHours_DayOfWeek_DayOfWeekId",
                        column: x => x.DayOfWeekId,
                        principalTable: "DayOfWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VhoWorkHours_JusticeUser_JusticeUserId",
                        column: x => x.JusticeUserId,
                        principalTable: "JusticeUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allocation_HearingId",
                table: "Allocation",
                column: "HearingId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocation_JusticeUserId",
                table: "Allocation",
                column: "JusticeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DayOfWeek_Day",
                table: "DayOfWeek",
                column: "Day",
                unique: true,
                filter: "[Day] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VhoNonAvailability_JusticeUserId",
                table: "VhoNonAvailability",
                column: "JusticeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VhoWorkHours_DayOfWeekId",
                table: "VhoWorkHours",
                column: "DayOfWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_VhoWorkHours_JusticeUserId",
                table: "VhoWorkHours",
                column: "JusticeUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allocation");

            migrationBuilder.DropTable(
                name: "VhoNonAvailability");

            migrationBuilder.DropTable(
                name: "VhoWorkHours");

            migrationBuilder.DropTable(
                name: "DayOfWeek");

            migrationBuilder.DropTable(
                name: "JusticeUser");
        }
    }
}
