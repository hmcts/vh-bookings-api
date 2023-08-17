SET XACT_ABORT ON
GO

CREATE OR ALTER PROC #HearingType_Create @id int, @name nvarchar(max), @caseTypeId int, @code nvarchar(max), @welshName nvarchar(max)
AS
BEGIN
    SET IDENTITY_INSERT HearingType ON
    INSERT INTO HearingType (Id, Name, CaseTypeId, Live, CreatedDate, UpdatedDate, Code, WelshName) VALUES (@id, @name, @caseTypeId, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @code, @welshName)
    SET IDENTITY_INSERT HearingType OFF
END
GO

CREATE OR ALTER PROC #CaseRole_Create @id int, @name nvarchar(max), @group int, @caseTypeId int
AS
BEGIN
    SET IDENTITY_INSERT CaseRole ON
    INSERT INTO CaseRole (Id, Name, [Group], CaseTypeId, CreatedDate, UpdatedDate) VALUES (@id, @name, @group, @caseTypeId, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
    SET IDENTITY_INSERT CaseRole OFF
END
GO

CREATE OR ALTER PROC #HearingRole_Create @id int, @name nvarchar(max), @userRoleId int, @caseRoleId int
AS
BEGIN
    SET IDENTITY_INSERT HearingRole ON
    INSERT INTO HearingRole (Id, Name, UserRoleId, CaseRoleId, Live, CreatedDate, UpdatedDate) VALUES (@id, @name, @userRoleId, @caseRoleId, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
    SET IDENTITY_INSERT HearingRole OFF
END
GO

BEGIN TRANSACTION

-- Jursidiction
DECLARE @JurisdictionId INT = 14
SET IDENTITY_INSERT Jurisdiction ON
INSERT INTO Jurisdiction (Id, Code, Name, IsLive, CreatedDate, UpdatedDate) VALUES (@JurisdictionId, 'Crime', 'Crime', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
SET IDENTITY_INSERT Jurisdiction OFF

-- Case Type
DECLARE @CaseTypeId INT = 55
SET IDENTITY_INSERT CaseType ON
INSERT INTO CaseType (Id, Name, CreatedDate, UpdatedDate, JurisdictionId, Live) VALUES (@CaseTypeId, 'Crime Crown Court', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @JurisdictionId, 1)
SET IDENTITY_INSERT CaseType OFF

-- Hearing Types
EXEC #HearingType_Create 332, 'ASS Appeal against Sentence', @CaseTypeId, 'ASS', 'Apêl yn erbyn Dedfryd'
EXEC #HearingType_Create 333, 'ACN Appeal against Conviction', @CaseTypeId, 'ACN', 'Apêl yn erbyn Euogfarn'
EXEC #HearingType_Create 334, 'ACS Appeal against Conviction and Sentence', @CaseTypeId, 'ACS', 'Apêl yn erbyn Euogfarn a Dedfryd'
EXEC #HearingType_Create 335, 'AEH Admissibility of Evidence - Half day', @CaseTypeId, 'AEH', 'Derbynioldeb tystiolaeth - Hanner diwrnod'
EXEC #HearingType_Create 336, 'AEW Admissibility of Evidence - Whole day', @CaseTypeId, 'AEW', 'Derbynioldeb tystiolaeth - Diwrnod cyfan'
EXEC #HearingType_Create 337, 'APN Application', @CaseTypeId, 'APN', 'Cais'
EXEC #HearingType_Create 338, 'BLA Bail Application ', @CaseTypeId, 'BLA', 'Cais am fechnÃ¯aeth'
EXEC #HearingType_Create 339, 'BOO Breach of Order', @CaseTypeId, 'BOO', 'Torri Amodau Gorchymyn'
EXEC #HearingType_Create 340, 'BRG Application to Rescind Grant of Bail', @CaseTypeId, 'BRG', 'Cais i Ddiddymu CaniatÃ¢d MechnÃ¯aeth'
EXEC #HearingType_Create 341, 'CB Committal for Breach', @CaseTypeId, 'CB', 'Traddodi am dorri amod'
EXEC #HearingType_Create 342, 'CCT Contempt of Court Proceedings', @CaseTypeId, 'CCT', 'Achos Dirmyg Llys'
EXEC #HearingType_Create 343, 'CH Confiscation Hearing', @CaseTypeId, 'CH', 'Gwrandawiad Atafaelu'
EXEC #HearingType_Create 344, 'CSE  Committal for Sentence', @CaseTypeId, 'CSE', 'Traddodi ar gyfer Dedfryd'
EXEC #HearingType_Create 345, 'CTL  Custody Time Limit Application', @CaseTypeId, 'CTL', 'Cais am Derfyn Amser yn y Ddalfa'
EXEC #HearingType_Create 346, 'EBW Execution of Bench Warrant', @CaseTypeId, 'EBW', 'Gweithredu Gwarant o''r Fainc'
EXEC #HearingType_Create 347, 'EBP Early Guilty Plea', @CaseTypeId, 'EBP', 'Ple Euog Cynnar'
EXEC #HearingType_Create 348, 'FCMG Further Case Management - General', @CaseTypeId, 'FCMG', 'Rheoli Achos Pellach - Cyffredinol'
EXEC #HearingType_Create 349, 'PONE Prosecution to offer no evidence', @CaseTypeId, 'PONE', 'Yr erlyniad i gynnig dim tystiolaeth'
EXEC #HearingType_Create 350, 'PTIE Pre-Trial to resolve issue of evidence', @CaseTypeId, 'PTIE', 'Cyn-treial i drafod cyflwyno tystiolaeth'
EXEC #HearingType_Create 351, 'PTP Plea and Trial Preparation', @CaseTypeId, 'PTP', 'Paratoi ar gyfer Pledio a Threial'
EXEC #HearingType_Create 352, 'PTR Pre-Trial Review', @CaseTypeId, 'PTR', 'Adolygiad Cyn-treial'
EXEC #HearingType_Create 353, 'SEN Sentence', @CaseTypeId, 'SEN', 'Dedfryd'
EXEC #HearingType_Create 354, 'TIS Trial of Issue', @CaseTypeId, 'TIS', 'Treial Pwnc Dadl'
EXEC #HearingType_Create 355, 'TRL Trial', @CaseTypeId, 'TRL', 'Treial'

-- Case Roles
DECLARE @CaseRoleId_Party INT = 356
DECLARE @CaseRoleId_Observer INT = 357
DECLARE @CaseRoleId_PanelMember INT = 358
DECLARE @CaseRoleGroup_Party INT = 26
DECLARE @CaseRoleGroup_Observer INT = 5
DECLARE @CaseRoleGroup_PanelMember INT = 6

EXEC #CaseRole_Create @CaseRoleId_Party, 'Party', @CaseRoleGroup_Party, @CaseTypeId
EXEC #CaseRole_Create @CaseRoleId_Observer, 'Observer', @CaseRoleGroup_Observer, @CaseTypeId
EXEC #CaseRole_Create @CaseRoleId_PanelMember, 'Panel Member', @CaseRoleGroup_PanelMember, @CaseTypeId

-- Hearing Roles
DECLARE @UserRoleId_Individual INT = 5
DECLARE @UserRoleId_Representative INT = 6
DECLARE @UserRoleId_Joh INT = 7

EXEC #HearingRole_Create 1163, 'Appellant', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1164, 'Appointee', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1165, 'Joint Party', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1166, 'Other Party', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1167, 'Respondent', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1168, 'Welfare Representative', @UserRoleId_Representative, @CaseRoleId_Party
EXEC #HearingRole_Create 1169, 'Legal Representative', @UserRoleId_Representative, @CaseRoleId_Party
EXEC #HearingRole_Create 1170, 'Barrister', @UserRoleId_Representative, @CaseRoleId_Party
EXEC #HearingRole_Create 1171, 'Interpreter', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1172, 'Representative', @UserRoleId_Representative, @CaseRoleId_Party
EXEC #HearingRole_Create 1173, 'Support', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1174, 'Applicant', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1175, 'Victim', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1176, 'Witness', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1177, 'Party', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1178, 'Defendant', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1179, 'Intermediaries', @UserRoleId_Representative, @CaseRoleId_Party
EXEC #HearingRole_Create 1180, 'Prosecutor', @UserRoleId_Representative, @CaseRoleId_Party
EXEC #HearingRole_Create 1181, 'Police', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1182, 'Defence Counsel', @UserRoleId_Representative, @CaseRoleId_Party
EXEC #HearingRole_Create 1183, 'Prosecution Counsel', @UserRoleId_Representative, @CaseRoleId_Party
EXEC #HearingRole_Create 1184, 'Claimant', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1185, 'Expert', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1186, 'Litigation Friend', @UserRoleId_Individual, @CaseRoleId_Party
EXEC #HearingRole_Create 1187, 'Observer', @UserRoleId_Individual, @CaseRoleId_Observer
EXEC #HearingRole_Create 1188, 'Panel Member', @UserRoleId_Joh, @CaseRoleId_PanelMember

COMMIT

SET XACT_ABORT OFF