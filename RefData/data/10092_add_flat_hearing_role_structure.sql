SET XACT_ABORT ON
GO;



CREATE OR ALTER PROC #HearingRole_CreateIfNotExist @hearingRoleName nvarchar(max), @userRoleId varchar(450)
As
BEGIN
    IF NOT EXISTS(SELECT * FROM VhBookings.dbo.HearingRole WHERE Name = @hearingRoleName AND UserRoleId = @userRoleId AND CaseRoleId IS NULL)
        BEGIN
            Print ('Adding new role ' + @hearingRoleName);
            INSERT INTO VhBookings.dbo.HearingRole (Name, UserRoleId, CaseRoleId, Live, CreatedDate, UpdatedDate) VALUES (@hearingRoleName, @userRoleId, NULL, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
            declare @id int = (SELECT SCOPE_IDENTITY());
            Print ('NEW ID IS ' + CAST(@id AS VARCHAR));
        END
END
GO;


BEGIN TRANSACTION;

DECLARE @Individual int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Individual');
DECLARE @Representative int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Representative');
DECLARE @JOH int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Judicial Office Holder');
DECLARE @Judge int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Judge');
DECLARE @StaffMember int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Staff Member');

EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Applicant', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Appellant', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Barrister', @userRoleId = @Representative;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Claimant', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Defendant', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Expert', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Welfare Representative', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Intermediaries', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Interpreter', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Joint Party', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Legal Representative', @userRoleId = @Representative;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Litigation Friend', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Observer', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Other Party', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Police', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Respondent', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Support', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Panel Member', @userRoleId = @JOH;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Witness', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Judge', @userRoleId = @Judge;
EXEC #HearingRole_CreateIfNotExist @hearingRoleName = 'Staff Member', @userRoleId = @StaffMember;

COMMIT
SET XACT_ABORT OFF