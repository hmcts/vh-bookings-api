SET XACT_ABORT ON
BEGIN TRANSACTION;

declare @genericCaseTypeId int;
SELECT @genericCaseTypeId = id FROM dbo.CaseType WHERE Name LIKE 'Generic';

UPDATE dbo.CaseType SET UpdatedDate = CURRENT_TIMESTAMP, ServiceId = N'VHGE1' WHERE Id = @genericCaseTypeId

UPDATE dbo.HearingType SET UpdatedDate = CURRENT_TIMESTAMP, Code = N'Hearing' WHERE CaseTypeId = @genericCaseTypeId and Name LIKE 'Hearing'
UPDATE dbo.HearingType SET UpdatedDate = CURRENT_TIMESTAMP, Code = N'DailyTest' WHERE CaseTypeId = @genericCaseTypeId and Name LIKE 'Daily Test'
UPDATE dbo.HearingType SET UpdatedDate = CURRENT_TIMESTAMP, Code = N'Demo' WHERE CaseTypeId = @genericCaseTypeId and Name LIKE 'Demo'
UPDATE dbo.HearingType SET UpdatedDate = CURRENT_TIMESTAMP, Code = N'Familiarisation' WHERE CaseTypeId = @genericCaseTypeId and Name LIKE 'Familiarisation'
UPDATE dbo.HearingType SET UpdatedDate = CURRENT_TIMESTAMP, Code = N'OneToOne' WHERE CaseTypeId = @genericCaseTypeId and Name LIKE 'One to one'
UPDATE dbo.HearingType SET UpdatedDate = CURRENT_TIMESTAMP, Code = N'Test' WHERE CaseTypeId = @genericCaseTypeId and Name LIKE 'Test'

SELECT * FROM CaseType WHERE Id = @genericCaseTypeId
SELECT * FROM HearingType WHERE CaseTypeId = @genericCaseTypeId

COMMIT
SET XACT_ABORT OFF