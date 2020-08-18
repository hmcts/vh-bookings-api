using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class Add_Endpoint_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Endpoint",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    SIP = table.Column<string>(nullable: false, type: "NVARCHAR(450)"),
                    PIN = table.Column<string>(nullable: false),
                    HearingId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Endpoint_Hearing_HearingId",
                        column: x => x.HearingId,
                        principalTable: "Hearing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_HearingId",
                table: "Endpoint",
                column: "HearingId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_SIP",
                table: "Endpoint",
                column: "SIP",
                unique: true,
                filter: "[SIP] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Endpoint");
        }
    }
}
