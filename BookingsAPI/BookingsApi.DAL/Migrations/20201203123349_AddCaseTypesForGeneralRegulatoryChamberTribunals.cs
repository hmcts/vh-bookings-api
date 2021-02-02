using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseTypesForGeneralRegulatoryChamberTribunals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 19, "GRC - Charity" },
                    { 20, "GRC - CRB" },
                    { 21, "GRC - DVSA" },
                    { 22, "GRC - Estate Agents" },
                    { 23, "GRC - Food" },
                    { 24, "GRC - Environment" },
                    { 25, "GRC - Gambling" },
                    { 26, "GRC - Immigration Services" },
                    { 27, "GRC - Information Rights" },
                    { 28, "GRC - Pensions Regulation" },
                    { 29, "GRC - Professional Regulations" },
                    { 30, "GRC - Query Jurisdiction" },
                    { 31, "GRC - Welfare of Animals" }
                });
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 75, "Case Management Hearing", 19 },
                    { 76, "Final Hearing", 19 },
                    { 77, "Costs Hearing", 19 },
                    { 78, "Case Management Hearing", 20 },
                    { 79, "Final Hearing", 20 },
                    { 80, "Costs Hearing", 20 },
                    { 81, "Case Management Hearing", 21 },
                    { 82, "Final Hearing", 21 },
                    { 83, "Costs Hearing", 21 },
                    { 84, "Case Management Hearing", 22 },
                    { 85, "Final Hearing", 22 },
                    { 86, "Costs Hearing", 22 },
                    { 87, "Case Management Hearing", 23 },
                    { 88, "Final Hearing", 23 },
                    { 89, "Costs Hearing", 23 },
                    { 90, "Case Management Hearing", 24 },
                    { 91, "Final Hearing", 24 },
                    { 92, "Costs Hearing", 24 },
                    { 93, "Case Management Hearing", 25 },
                    { 94, "Final Hearing", 25 },
                    { 95, "Costs Hearing", 25 },
                    { 96, "Case Management Hearing", 26 },
                    { 97, "Final Hearing", 26 },
                    { 98, "Costs Hearing", 26 },
                    { 99, "Case Management Hearing", 27 },
                    { 100, "Final Hearing", 27 },
                    { 101, "Costs Hearing", 27 },
                    { 102, "Case Management Hearing", 28 },
                    { 103, "Final Hearing", 28 },
                    { 104, "Costs Hearing", 28 },
                    { 105, "Case Management Hearing", 29 },
                    { 106, "Final Hearing", 29 },
                    { 107, "Costs Hearing", 29 },
                    { 108, "Case Management Hearing", 30 },
                    { 109, "Final Hearing", 30 },
                    { 110, "Costs Hearing", 30 },
                    { 111, "Case Management Hearing", 31 },
                    { 112, "Final Hearing", 31 },
                    { 113, "Costs Hearing", 31 }
                });
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 90, "Applicant", (int) CaseRoleGroup.Applicant, 19 },
                    { 91, "Respondent", (int) CaseRoleGroup.Respondent, 19 },
                    { 92, "Judge", (int) CaseRoleGroup.Judge, 19 },
                    { 93, "Panel Member", (int) CaseRoleGroup.PanelMember, 19 },
                    { 94, "Observer", (int) CaseRoleGroup.Observer, 19 },

                    { 95, "Applicant", (int) CaseRoleGroup.Applicant, 20 },
                    { 96, "Respondent", (int) CaseRoleGroup.Respondent, 20 },
                    { 97, "Judge", (int) CaseRoleGroup.Judge, 20 },
                    { 98, "Panel Member", (int) CaseRoleGroup.PanelMember, 20 },
                    { 99, "Observer", (int) CaseRoleGroup.Observer, 20 },

                    { 100, "Applicant", (int) CaseRoleGroup.Applicant, 21 },
                    { 101, "Respondent", (int) CaseRoleGroup.Respondent, 21 },
                    { 102, "Judge", (int) CaseRoleGroup.Judge, 21 },
                    { 103, "Panel Member", (int) CaseRoleGroup.PanelMember, 21 },
                    { 104, "Observer", (int) CaseRoleGroup.Observer, 21 },

                    { 105, "Applicant", (int) CaseRoleGroup.Applicant, 22 },
                    { 106, "Respondent", (int) CaseRoleGroup.Respondent, 22 },
                    { 107, "Judge", (int) CaseRoleGroup.Judge, 22 },
                    { 108, "Panel Member", (int) CaseRoleGroup.PanelMember, 22 },
                    { 109, "Observer", (int) CaseRoleGroup.Observer, 22 },

                    { 110, "Applicant", (int) CaseRoleGroup.Applicant, 23 },
                    { 111, "Respondent", (int) CaseRoleGroup.Respondent, 23 },
                    { 112, "Judge", (int) CaseRoleGroup.Judge, 23 },
                    { 113, "Panel Member", (int) CaseRoleGroup.PanelMember, 23 },
                    { 114, "Observer", (int) CaseRoleGroup.Observer, 23 },

                    { 115, "Applicant", (int) CaseRoleGroup.Applicant, 24 },
                    { 116, "Respondent", (int) CaseRoleGroup.Respondent, 24 },
                    { 117, "Judge", (int) CaseRoleGroup.Judge, 24 },
                    { 118, "Panel Member", (int) CaseRoleGroup.PanelMember, 24 },
                    { 119, "Observer", (int) CaseRoleGroup.Observer, 24 },

                    { 120, "Applicant", (int) CaseRoleGroup.Applicant, 25 },
                    { 121, "Respondent", (int) CaseRoleGroup.Respondent, 25 },
                    { 122, "Judge", (int) CaseRoleGroup.Judge, 25 },
                    { 123, "Panel Member", (int) CaseRoleGroup.PanelMember, 25 },
                    { 124, "Observer", (int) CaseRoleGroup.Observer, 25 },

                    { 125, "Applicant", (int) CaseRoleGroup.Applicant, 26 },
                    { 126, "Respondent", (int) CaseRoleGroup.Respondent, 26 },
                    { 127, "Judge", (int) CaseRoleGroup.Judge, 26 },
                    { 128, "Panel Member", (int) CaseRoleGroup.PanelMember, 26 },
                    { 129, "Observer", (int) CaseRoleGroup.Observer, 26 },

                    { 130, "Applicant", (int) CaseRoleGroup.Applicant, 27 },
                    { 131, "Respondent", (int) CaseRoleGroup.Respondent, 27 },
                    { 132, "Judge", (int) CaseRoleGroup.Judge, 27 },
                    { 133, "Panel Member", (int) CaseRoleGroup.PanelMember, 27 },
                    { 134, "Observer", (int) CaseRoleGroup.Observer, 27 },

                    { 135, "Applicant", (int) CaseRoleGroup.Applicant, 28 },
                    { 136, "Respondent", (int) CaseRoleGroup.Respondent, 28 },
                    { 137, "Judge", (int) CaseRoleGroup.Judge, 28 },
                    { 138, "Panel Member", (int) CaseRoleGroup.PanelMember, 28 },
                    { 139, "Observer", (int) CaseRoleGroup.Observer, 28 },

                    { 140, "Applicant", (int) CaseRoleGroup.Applicant, 29 },
                    { 141, "Respondent", (int) CaseRoleGroup.Respondent, 29 },
                    { 142, "Judge", (int) CaseRoleGroup.Judge, 29 },
                    { 143, "Panel Member", (int) CaseRoleGroup.PanelMember, 29 },
                    { 144, "Observer", (int) CaseRoleGroup.Observer, 29 },

                    { 145, "Applicant", (int) CaseRoleGroup.Applicant, 30 },
                    { 146, "Respondent", (int) CaseRoleGroup.Respondent, 30 },
                    { 147, "Judge", (int) CaseRoleGroup.Judge, 30 },
                    { 148, "Panel Member", (int) CaseRoleGroup.PanelMember, 30 },
                    { 149, "Observer", (int) CaseRoleGroup.Observer, 30 },

                    { 150, "Applicant", (int) CaseRoleGroup.Applicant, 31 },
                    { 151, "Respondent", (int) CaseRoleGroup.Respondent, 31 },
                    { 152, "Judge", (int) CaseRoleGroup.Judge, 31 },
                    { 153, "Panel Member", (int) CaseRoleGroup.PanelMember, 31 },
                    { 154, "Observer", (int) CaseRoleGroup.Observer, 31 },
                });
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    { 214, "Litigant in person", 5, 90 },
                    { 215, "Representative", 6, 90 },
                    { 216, "Witness", 5, 90 },
                    { 217, "Litigant in person", 5, 91 },
                    { 218, "Representative", 6, 91 },
                    { 219, "Witness", 5, 91 },
                    { 220, "Judge", 4, 92 },
                    { 221, "Panel Member", 5, 93 },
                    { 222, "Observer", 5, 94 },

                    { 223, "Litigant in person", 5, 95 },
                    { 224, "Representative", 6, 95 },
                    { 225, "Witness", 5, 95 },
                    { 226, "Litigant in person", 5, 96 },
                    { 227, "Representative", 6, 96 },
                    { 228, "Witness", 5, 96 },
                    { 229, "Judge", 4, 97 },
                    { 230, "Panel Member", 5, 98 },
                    { 231, "Observer", 5, 99 },

                    { 232, "Litigant in person", 5, 100 },
                    { 233, "Representative", 6, 100 },
                    { 234, "Witness", 5, 100 },
                    { 235, "Litigant in person", 5, 101 },
                    { 236, "Representative", 6, 101 },
                    { 237, "Witness", 5, 101 },
                    { 238, "Judge", 4, 102 },
                    { 239, "Panel Member", 5, 103 },
                    { 240, "Observer", 5, 104 },

                    { 241, "Litigant in person", 5, 105 },
                    { 242, "Representative", 6, 105 },
                    { 243, "Witness", 5, 105 },
                    { 244, "Litigant in person", 5, 106 },
                    { 245, "Representative", 6, 106 },
                    { 246, "Witness", 5, 106 },
                    { 247, "Judge", 4, 107 },
                    { 248, "Panel Member", 5, 108 },
                    { 249, "Observer", 5, 109 },

                    { 250, "Litigant in person", 5, 110 },
                    { 251, "Representative", 6, 110 },
                    { 252, "Witness", 5, 110 },
                    { 253, "Litigant in person", 5, 111 },
                    { 254, "Representative", 6, 111 },
                    { 255, "Witness", 5, 111 },
                    { 256, "Judge", 4, 112 },
                    { 257, "Panel Member", 5, 113 },
                    { 258, "Observer", 5, 114 },

                    { 259, "Litigant in person", 5, 115 },
                    { 260, "Representative", 6, 115 },
                    { 261, "Witness", 5, 115 },
                    { 262, "Litigant in person", 5, 116 },
                    { 263, "Representative", 6, 116 },
                    { 264, "Witness", 5, 116 },
                    { 265, "Judge", 4, 117 },
                    { 266, "Panel Member", 5, 118 },
                    { 267, "Observer", 5, 119 },

                    { 268, "Litigant in person", 5, 120 },
                    { 269, "Representative", 6, 120 },
                    { 270, "Witness", 5, 120 },
                    { 271, "Litigant in person", 5, 121 },
                    { 272, "Representative", 6, 121 },
                    { 273, "Witness", 5, 121 },
                    { 274, "Judge", 4, 122 },
                    { 275, "Panel Member", 5, 123 },
                    { 276, "Observer", 5, 124 },

                    { 277, "Litigant in person", 5, 125 },
                    { 278, "Representative", 6, 125 },
                    { 279, "Witness", 5, 125 },
                    { 280, "Litigant in person", 5, 126 },
                    { 281, "Representative", 6, 126 },
                    { 282, "Witness", 5, 126 },
                    { 283, "Judge", 4, 127 },
                    { 284, "Panel Member", 5, 128 },
                    { 285, "Observer", 5, 129 },

                    { 286, "Litigant in person", 5, 130 },
                    { 287, "Representative", 6, 130 },
                    { 288, "Witness", 5, 130 },
                    { 289, "Litigant in person", 5, 131 },
                    { 290, "Representative", 6, 131 },
                    { 291, "Witness", 5, 131 },
                    { 292, "Judge", 4, 132 },
                    { 293, "Panel Member", 5, 133 },
                    { 294, "Observer", 5, 134 },

                    { 295, "Litigant in person", 5, 135 },
                    { 296, "Representative", 6, 135 },
                    { 297, "Witness", 5, 135 },
                    { 298, "Litigant in person", 5, 136 },
                    { 299, "Representative", 6, 136 },
                    { 300, "Witness", 5, 136 },
                    { 301, "Judge", 4, 137 },
                    { 302, "Panel Member", 5, 138 },
                    { 303, "Observer", 5, 139 },

                    { 304, "Litigant in person", 5, 140 },
                    { 305, "Representative", 6, 140 },
                    { 306, "Witness", 5, 140 },
                    { 307, "Litigant in person", 5, 141 },
                    { 308, "Representative", 6, 141 },
                    { 309, "Witness", 5, 141 },
                    { 310, "Judge", 4, 142 },
                    { 311, "Panel Member", 5, 143 },
                    { 312, "Observer", 5, 144 },

                    { 313, "Litigant in person", 5, 145 },
                    { 314, "Representative", 6, 145 },
                    { 315, "Witness", 5, 145 },
                    { 316, "Litigant in person", 5, 146 },
                    { 317, "Representative", 6, 146 },
                    { 318, "Witness", 5, 146 },
                    { 319, "Judge", 4, 147 },
                    { 320, "Panel Member", 5, 148 },
                    { 321, "Observer", 5, 149 },

                    { 322, "Litigant in person", 5, 150 },
                    { 323, "Representative", 6, 150 },
                    { 324, "Witness", 5, 150 },
                    { 325, "Litigant in person", 5, 151 },
                    { 326, "Representative", 6, 151 },
                    { 327, "Witness", 5, 151 },
                    { 328, "Judge", 4, 152 },
                    { 329, "Panel Member", 5, 153 },
                    { 330, "Observer", 5, 154 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 214; hearingRoleID <= 330; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }
            for (int CaseRoleRoleID = 90; CaseRoleRoleID <= 154; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }
            for (int HearingTypeId = 75; HearingTypeId <= 113; HearingTypeId++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", HearingTypeId);
            }
            for (int CaseTypeId = 19; CaseTypeId <= 31; CaseTypeId++)
            {
                migrationBuilder.DeleteData("CaseType", "Id", CaseTypeId);
            }
        }
    }
}
