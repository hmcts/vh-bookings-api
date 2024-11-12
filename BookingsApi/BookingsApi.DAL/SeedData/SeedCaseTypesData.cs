namespace BookingsApi.DAL.SeedData
{
    public class SeedCaseTypesData
    {
        public void Up(MigrationBuilder migrationBuilder)
        {
            AddHearingVenues(migrationBuilder);
            AddUserRoles(migrationBuilder);
            AddCaseTypes(migrationBuilder);
            AddCaseRoles(migrationBuilder);
            AddHearingTypes(migrationBuilder);
            AddHearingRoles(migrationBuilder);
        }

        private void AddHearingVenues(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] {"Id", "Name"},
                values: new object[,]
                {
                    {1, "Birmingham Civil and Family Justice Centre"},
                    {2, "Manchester Civil and Family Justice Centre"}
                });
        }

        public void AddUserRoles(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(UserRole),
                columns: new[] {"Id", "Name"},
                values: new object[,]
                {
                    {1, "Case admin"},
                    {2, "Video hearings officer"},
                    {3, "Hearing facilitation support (CTRT Clerk)"},
                    {4, "Judge"},
                    {5, "Individual"},
                    {6, "Representative"}
                });
        }
        
        public void AddCaseTypes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseType),
                columns: new[] {"Id", "Name"},
                values: new object[,]
                {
                    {1, "Civil Money Claims"},
                    {2, "Financial Remedy"}
                });
        }

        public void AddCaseRoles(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] {"Id", "Name", "Group", "CaseTypeId"},
                values: new object[,]
                {
                    {1, "Claimant", (int) CaseRoleGroup.Claimant, 1},
                    {2, "Defendant", (int) CaseRoleGroup.Defendant, 1},
                    {3, "Applicant", (int) CaseRoleGroup.Applicant, 2},
                    {4, "Respondent", (int) CaseRoleGroup.Respondent, 2}
                });
        }

        public void AddHearingTypes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] {"Id", "Name", "CaseTypeId"},
                values: new object[,]
                {
                    {1, "Application to Set Judgment Aside", 1},
                    {2, "First Directions Appointment", 2},
                });
        }

        public void AddHearingRoles(MigrationBuilder migrationBuilder)
        {   
            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] {"Id", "Name", "CaseRoleId", "UserRoleId",},
                values: new object[,]
                {
                    // Case role: Claimant
                    {1, "Claimant LIP", 1,5},
                    {2, "Solicitor", 1, 6},
                    {3, "McKenzie Friend", 1, 6},

                    // Case role: Defendant
                    {4, "Defendant LIP", 2, 5},
                    {5, "Solicitor", 2, 6},
                    {6, "McKenzie Friend", 2, 6},
                    
                    // Case role: Applicant
                    {7, "Applicant LIP", 3,5},
                    {8, "Solicitor", 3, 6},
                    {9, "McKenzie Friend", 3, 6},

                    // Case role: Respondent
                    {10, "Applicant LIP", 4,5},
                    {11, "Solicitor", 4, 6},
                    {12, "McKenzie Friend", 4, 6}
                });
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"delete from {nameof(HearingRole)}");
            migrationBuilder.Sql($"delete from {nameof(CaseRole)}");
            migrationBuilder.Sql($"delete from {nameof(HearingType)}");
            migrationBuilder.Sql($"delete from {nameof(CaseType)}");
            migrationBuilder.Sql($"delete from {nameof(UserRole)}");
            migrationBuilder.Sql($"delete from {nameof(HearingVenue)}");
            
            migrationBuilder.Sql($"dbcc checkident('{nameof(HearingRole)}',reseed,0)");
            migrationBuilder.Sql($"dbcc checkident('{nameof(CaseType)}',reseed,0)");
            migrationBuilder.Sql($"dbcc checkident('{nameof(UserRole)}',reseed,0)");
            migrationBuilder.Sql($"dbcc checkident('{nameof(CaseRole)}',reseed,0)");
            migrationBuilder.Sql($"dbcc checkident('{nameof(HearingType)}',reseed,0)");
        }
    }
}