using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddInterpreterHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                values: new object[,]
                {
                    {379, "Interpreter ", 5, 1},
                    {380, "Interpreter ", 5, 2},
                    {381, "Interpreter ", 5, 3},
                    {382, "Interpreter ", 5, 4},
                    {383, "Interpreter ", 5, 7},
                    {384, "Interpreter ", 5, 8},
                    {385, "Interpreter ", 5, 10},
                    {386, "Interpreter ", 5, 11},
                    {387, "Interpreter ", 5, 13},
                    {388, "Interpreter ", 5, 14},
                    {389, "Interpreter ", 5, 16},
                    {390, "Interpreter ", 5, 17},
                    {391, "Interpreter ", 5, 19},
                    {392, "Interpreter ", 5, 20},
                    {393, "Interpreter ", 5, 22},
                    {394, "Interpreter ", 5, 23},
                    {395, "Interpreter ", 5, 25},
                    {396, "Interpreter ", 5, 26},
                    {397, "Interpreter ", 5, 28},
                    {398, "Interpreter ", 5, 29},
                    {399, "Interpreter ", 5, 31},
                    {400, "Interpreter ", 5, 32},
                    {401, "Interpreter ", 5, 34},
                    {402, "Interpreter ", 5, 35},
                    {403, "Interpreter ", 5, 37},
                    {404, "Interpreter ", 5, 38},
                    {405, "Interpreter ", 5, 40},
                    {406, "Interpreter ", 5, 41},
                    {407, "Interpreter ", 5, 43},
                    {408, "Interpreter ", 5, 44},
                    {409, "Interpreter ", 5, 46},
                    {410, "Interpreter ", 5, 47},
                    {411, "Interpreter ", 5, 81},
                    {412, "Interpreter ", 5, 83},
                    {413, "Interpreter ", 5, 84},
                    {414, "Interpreter ", 5, 90},
                    {415, "Interpreter ", 5, 91},
                    {416, "Interpreter ", 5, 95},
                    {417, "Interpreter ", 5, 96},
                    {418, "Interpreter ", 5, 100},
                    {419, "Interpreter ", 5, 101},
                    {420, "Interpreter ", 5, 105},
                    {421, "Interpreter ", 5, 106},
                    {422, "Interpreter ", 5, 110},
                    {423, "Interpreter ", 5, 111},
                    {424, "Interpreter ", 5, 115},
                    {425, "Interpreter ", 5, 116},
                    {426, "Interpreter ", 5, 120},
                    {427, "Interpreter ", 5, 121},
                    {428, "Interpreter ", 5, 125},
                    {429, "Interpreter ", 5, 126},
                    {430, "Interpreter ", 5, 130},
                    {431, "Interpreter ", 5, 131},
                    {432, "Interpreter ", 5, 135},
                    {433, "Interpreter ", 5, 136},
                    {434, "Interpreter ", 5, 140},
                    {435, "Interpreter ", 5, 141},
                    {436, "Interpreter ", 5, 145},
                    {437, "Interpreter ", 5, 146},
                    {438, "Interpreter ", 5, 150},
                    {439, "Interpreter ", 5, 151},
                    {440, "Interpreter ", 5, 155},
                    {441, "Interpreter ", 5, 160},
                    {442, "Interpreter ", 5, 161},
                    {443, "Interpreter ", 5, 162},
                    {444, "Interpreter ", 5, 163},
                    {445, "Interpreter ", 5, 164},
                    {446, "Interpreter ", 5, 165},
                    {447, "Interpreter ", 5, 166},
                    {448, "Interpreter ", 5, 167},
                    {449, "Interpreter ", 5, 168},
                    {450, "Interpreter ", 5, 169},
                    {451, "Interpreter ", 5, 170},
                    {452, "Interpreter ", 5, 171},
                    {453, "Interpreter ", 5, 172},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 379; hearingRoleId <= 453; hearingRoleId++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleId);
            }
        }
    }
}
