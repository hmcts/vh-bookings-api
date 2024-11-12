namespace BookingsApi.DAL.SeedData;

//TODO: Remove CaseRole seeding as part of https://tools.hmcts.net/jira/browse/VIH-10899
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

    private static void AddUserRoles(MigrationBuilder migrationBuilder)
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
        
    private static void AddCaseTypes(MigrationBuilder migrationBuilder)
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

    private static void AddCaseRoles(MigrationBuilder migrationBuilder)
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

    private static void AddHearingRoles(MigrationBuilder migrationBuilder)
    {   
        const string solicitor = "Solicitor";
        const string mkFriend  = "McKenzie Friend";
            
        migrationBuilder.InsertData(
            table: nameof(HearingRole),
            ["Id", "Name", "UserRoleId"],
            values: new object[,]
            {
                // Case role: Claimant
                {1, "Claimant LIP", 5},
                {2, solicitor, 6},
                {3, mkFriend, 6},

                // Case role: Defendant
                {4, "Defendant LIP", 5},
                {5, solicitor, 6},
                {6, mkFriend, 6},
                    
                // Case role: Applicant
                {7, "Applicant LIP" ,5},
                {8, solicitor, 6},
                {9, mkFriend, 6},

                // Case role: Respondent
                {10, "Applicant LIP", 5},
                {11, solicitor, 6},
                {12, mkFriend, 6}
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