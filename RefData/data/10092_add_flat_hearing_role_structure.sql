SET XACT_ABORT ON
GO;



CREATE OR ALTER PROC #HearingRole_CreateIfNotExist @id int, @hearingRoleName nvarchar(max), @userRoleId varchar(450)
As
BEGIN
    IF NOT EXISTS(SELECT * FROM VhBookings.dbo.HearingRole WHERE Name = @hearingRoleName AND UserRoleId = @userRoleId AND CaseRoleId IS NULL)
        BEGIN
            Print ('Adding new role ' + @hearingRoleName);
            SET IDENTITY_INSERT VhBookings.dbo.HearingRole ON
            INSERT INTO VhBookings.dbo.HearingRole (Id, Name, UserRoleId, CaseRoleId, Live, CreatedDate, UpdatedDate) VALUES (@id, @hearingRoleName, @userRoleId, NULL, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
            SET IDENTITY_INSERT VhBookings.dbo.HearingRole OFF
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

EXEC #HearingRole_CreateIfNotExist @id = 1191, @hearingRoleName = 'Applicant', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1192, @hearingRoleName = 'Appellant', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1193, @hearingRoleName = 'Barrister', @userRoleId = @Representative;
EXEC #HearingRole_CreateIfNotExist @id = 1194, @hearingRoleName = 'Claimant', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1195, @hearingRoleName = 'Defendant', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1196, @hearingRoleName = 'Expert', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1197, @hearingRoleName = 'Welfare Representative', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1198, @hearingRoleName = 'Intermediaries', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1199, @hearingRoleName = 'Interpreter', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1200, @hearingRoleName = 'Joint Party', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1201, @hearingRoleName = 'Legal Representative', @userRoleId = @Representative;
EXEC #HearingRole_CreateIfNotExist @id = 1202, @hearingRoleName = 'Litigation Friend', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1203, @hearingRoleName = 'Observer', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1204, @hearingRoleName = 'Other Party', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1205, @hearingRoleName = 'Police', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1206, @hearingRoleName = 'Respondent', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1207, @hearingRoleName = 'Support', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1208, @hearingRoleName = 'Panel Member', @userRoleId = @JOH;
EXEC #HearingRole_CreateIfNotExist @id = 1209, @hearingRoleName = 'Witness', @userRoleId = @Individual;
EXEC #HearingRole_CreateIfNotExist @id = 1210, @hearingRoleName = 'Judge', @userRoleId = @Judge;
EXEC #HearingRole_CreateIfNotExist @id = 1211, @hearingRoleName = 'Staff Member', @userRoleId = @StaffMember;

COMMIT
SET XACT_ABORT OFF