SET XACT_ABORT ON
GO

CREATE OR ALTER PROC #HearingRole_CreateIfNotExist @id int, @hearingRoleName nvarchar(max), @userRoleId int
As
BEGIN
    IF NOT EXISTS(SELECT * FROM VhBookings.dbo.HearingRole WHERE Name = @hearingRoleName AND UserRoleId = @userRoleId AND CaseRoleId IS NULL)
        BEGIN
            Print ('Adding new role ' + @hearingRoleName)
            SET IDENTITY_INSERT VhBookings.dbo.HearingRole ON
            INSERT INTO VhBookings.dbo.HearingRole (Id, Name, UserRoleId, CaseRoleId, Live, CreatedDate, UpdatedDate) VALUES (@id, @hearingRoleName, @userRoleId, NULL, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
            SET IDENTITY_INSERT VhBookings.dbo.HearingRole OFF
            Print ('NEW ID IS ' + CAST(@id AS VARCHAR))
        END
END
GO

CREATE OR ALTER PROC #HearingRole_Update @name nvarchar(max), @code nvarchar(450), @welshName nvarchar(max), @userRoleId int
AS
BEGIN
    IF EXISTS (SELECT TOP 1 1 FROM HearingRole WHERE Name = @name AND CaseRoleId IS NULL)
    BEGIN
        UPDATE HearingRole
        SET Code = @code, WelshName = @welshName, UserRoleId = @userRoleId
        WHERE Name = @name AND CaseRoleId IS NULL
    END
    ELSE
    BEGIN
        Print ('WARNING! Could not find hearing role with name: ' + @name)
    END
END
GO

BEGIN TRANSACTION

DECLARE @Individual int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Individual')
DECLARE @Representative int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Representative')
DECLARE @JOH int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Judicial Office Holder')
DECLARE @Judge int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Judge')
DECLARE @StaffMember int = (SELECT ID From VhBookings.dbo.UserRole WHERE Name = 'Staff Member')

-- Add missing roles
EXEC #HearingRole_CreateIfNotExist @id = 1212, @hearingRoleName = 'Appointee', @userRoleId = @Individual
EXEC #HearingRole_CreateIfNotExist @id = 1213, @hearingRoleName = 'Defence Counsel', @userRoleId = @Representative
EXEC #HearingRole_CreateIfNotExist @id = 1214, @hearingRoleName = 'Party', @userRoleId = @Individual
EXEC #HearingRole_CreateIfNotExist @id = 1215, @hearingRoleName = 'Prosecution Counsel', @userRoleId = @Representative
EXEC #HearingRole_CreateIfNotExist @id = 1216, @hearingRoleName = 'Prosecutor', @userRoleId = @Representative
EXEC #HearingRole_CreateIfNotExist @id = 1217, @hearingRoleName = 'Representative', @userRoleId = @Representative
EXEC #HearingRole_CreateIfNotExist @id = 1218, @hearingRoleName = 'Victim', @userRoleId = @Individual

-- Populate code and welsh name, and update the user role
EXEC #HearingRole_Update 'Appellant', 'APEL', 'Apelydd', @Individual
EXEC #HearingRole_Update 'Applicant', 'APPL', 'Ceisydd', @Individual
EXEC #HearingRole_Update 'Appointee', 'APIN', 'Penodai', @Individual
EXEC #HearingRole_Update 'Barrister', 'BARR', 'Bargyfreithiwr', @Representative
EXEC #HearingRole_Update 'Claimant', 'CLAI', 'Hawlydd', @Individual
EXEC #HearingRole_Update 'Defence Counsel', 'DECO', 'Cwnsler yr Amddiffyniad', @Representative
EXEC #HearingRole_Update 'Defendant', 'DEFE', 'Diffynnydd', @Individual
EXEC #HearingRole_Update 'Expert', 'EXPR', 'Arbenigwr', @Individual
EXEC #HearingRole_Update 'Intermediaries', 'INTE', 'Cyfryngwyr', @Representative
EXEC #HearingRole_Update 'Interpreter', 'INTP', 'Cyfieithydd ar y Pryd', @Individual
EXEC #HearingRole_Update 'Joint Party', 'JOPA', 'Parti ar y Cyd', @Individual
EXEC #HearingRole_Update 'Legal Representative', 'LGRP', 'Cynrychiolydd Cyfreithiol', @Representative
EXEC #HearingRole_Update 'Litigation Friend', 'LIFR', 'Cyfaill Cyfreitha', @Individual
EXEC #HearingRole_Update 'Observer', 'OBSV', 'Sylwebydd', @Individual
EXEC #HearingRole_Update 'Other Party', 'OTPA', 'Parti Arall', @Individual
EXEC #HearingRole_Update 'Party', 'PART', 'Parti', @Individual
EXEC #HearingRole_Update 'Police', 'POLI', 'Yr Heddlu', @Individual
EXEC #HearingRole_Update 'Prosecution Counsel', 'PRCO', 'Cwnsler yr Erlyniad', @Representative
EXEC #HearingRole_Update 'Prosecutor', 'PROS', 'Erlynydd', @Representative
EXEC #HearingRole_Update 'Representative', 'RPTT', 'Cynrychiolydd', @Representative
EXEC #HearingRole_Update 'Respondent', 'RESP', 'Atebydd', @Individual
EXEC #HearingRole_Update 'Support', 'SUPP', 'Cefnogaeth', @Individual
EXEC #HearingRole_Update 'Victim', 'VICT', 'Dioddefwr', @Individual
EXEC #HearingRole_Update 'Welfare Representative', 'WERP', 'Cynrychiolydd Lles', @Representative
EXEC #HearingRole_Update 'Witness', 'WITN', 'Tyst', @Individual
EXEC #HearingRole_Update 'Judge', 'JUDG', 'Barnwr', @Judge
EXEC #HearingRole_Update 'Panel Member', 'PANL', 'Aelod o''r Panel', @JOH
EXEC #HearingRole_Update 'Staff Member', 'STAF', 'Aelod o staff', @StaffMember

COMMIT

SET XACT_ABORT OFF