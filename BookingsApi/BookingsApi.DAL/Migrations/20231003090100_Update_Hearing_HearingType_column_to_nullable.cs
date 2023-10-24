using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class Update_Hearing_HearingType_column_to_nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hearing_HearingType_HearingTypeId",
                table: "Hearing");

            migrationBuilder.AlterColumn<int>(
                name: "HearingTypeId",
                table: "Hearing",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Hearing_HearingType_HearingTypeId",
                table: "Hearing",
                column: "HearingTypeId",
                principalTable: "HearingType",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hearing_HearingType_HearingTypeId",
                table: "Hearing");

            migrationBuilder.AlterColumn<int>(
                name: "HearingTypeId",
                table: "Hearing",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hearing_HearingType_HearingTypeId",
                table: "Hearing",
                column: "HearingTypeId",
                principalTable: "HearingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
