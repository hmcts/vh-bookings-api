SET XACT_ABORT ON
GO

CREATE PROCEDURE #UPDATE_CASE_TYPES_V10192 @oldName varchar(max), @name varchar(max), @serviceId varchar(max), @JurisdictionId INT
AS
BEGIN
    UPDATE CaseType
    SET Name = @name, UpdatedDate = CURRENT_TIMESTAMP, JurisdictionId = @JurisdictionId, ServiceId = @serviceId
    WHERE Name = @oldName
END
GO
CREATE PROCEDURE #CREATE_CASE_TYPES_V10192 @CaseTypeId INT, @name varchar(max), @serviceId varchar(max), @JurisdictionId INT
AS
BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.CaseType WHERE Id = @CaseTypeId)
    INSERT INTO CaseType (Id, Name, CreatedDate, UpdatedDate, JurisdictionId, Live, ServiceId)
    VALUES (@CaseTypeId, @name, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @JurisdictionId, 1, @serviceId)
END
GO

BEGIN TRANSACTION
BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.Jurisdiction WHERE Name = 'Tribunals')
    INSERT INTO Jurisdiction (Code, Name, CreatedDate, UpdatedDate, IsLive)
    VALUES ('Tribunals', 'Tribunals', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1)
END
DECLARE @JurisdictionId INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Tribunals')
SET IDENTITY_INSERT CaseType ON
--UPDATE
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Charity', 'Charity', 'BAA2', @JurisdictionId; --BAA2
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Environment', 'Environment', 'BAA5', @JurisdictionId; --BAA5
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Estate Agents', 'Estate Agents', 'BAA6',  @JurisdictionId;  --BAA6
--CREATE
EXEC #CREATE_CASE_TYPES_V10192 67, 'Examination Boards', 'BAA7', @JurisdictionId; --BAA7
--UPDATE
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Gambling', 'Gambling', 'BAA8', @JurisdictionId; --BAA8
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Immigration Services', 'Immigration Services', 'BAA9', @JurisdictionId; --BAA9
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Information Rights', 'Information Rights', 'BAB1', @JurisdictionId; --BAB1
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - DVSA', 'Transport', 'BAB3', @JurisdictionId; --BAB3
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - CRB', 'Community Right to Bid', 'BAB4', @JurisdictionId; --BAB4
--CREATE
EXEC #CREATE_CASE_TYPES_V10192 68, 'Electronic Communications, Postal Services & Network Information Systems', 'BAB5', @JurisdictionId; --BAB5
--UPDATE
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Food', 'Food Safety', 'BAB6', @JurisdictionId; --BAB6
--CREATE
EXEC #CREATE_CASE_TYPES_V10192 69, 'Individual Electoral Registration', 'BAB7', @JurisdictionId; --BAB7
EXEC #CREATE_CASE_TYPES_V10192 70, 'Licensing and Standards', 'BAB8', @JurisdictionId; --BAB8
--UPDATE
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Pensions Regulation', 'Pensions', 'BAB9', @JurisdictionId; --BAB9
EXEC #UPDATE_CASE_TYPES_V10192 'GRC - Welfare of Animals', 'Welfare of Animals', 'BAC1', @JurisdictionId; --BAC1
EXEC #UPDATE_CASE_TYPES_V10192 'SSCS Tribunal', 'Social Security and Child Support', 'BBA3', @JurisdictionId; --BBA3
--CREATE
EXEC #CREATE_CASE_TYPES_V10192 71, 'Primary Health Lists', 'BCA3', @JurisdictionId; --BCA3
--UPDATE
EXEC #UPDATE_CASE_TYPES_V10192 'Special Educational Needs and Disability', 'Special Educational Needs', 'BCA4',  @JurisdictionId; --BCA4
EXEC #UPDATE_CASE_TYPES_V10192 'Tax', 'Tax Appeals', 'BDA2', @JurisdictionId; --BDA2
EXEC #UPDATE_CASE_TYPES_V10192 'Immigration and Asylum', 'Immigration and Asylum Appeals', 'BFA1', @JurisdictionId; --BFA1
--CREATE
EXEC #CREATE_CASE_TYPES_V10192 72, 'MP’s Expenses', 'BDA1', @JurisdictionId; --BDA1
EXEC #CREATE_CASE_TYPES_V10192 73, 'Agricultural Land and Drainage', 'BGA1', @JurisdictionId; --BGA1
EXEC #CREATE_CASE_TYPES_V10192 74, 'Residential Property', 'BGA3', @JurisdictionId; --BGA3
--UPDATE
EXEC #UPDATE_CASE_TYPES_V10192 'Employment Tribunal', 'Employment Claims', 'BHA1',  @JurisdictionId; --BHA1
--CREATE
EXEC #CREATE_CASE_TYPES_V10192 75, 'Gangmasters Licensing Appeals', 'BHA2', @JurisdictionId; --BHA2
EXEC #CREATE_CASE_TYPES_V10192 76, 'Reserve Forces Appeal Tribunal', 'BHA3', @JurisdictionId; --BHA3
SET IDENTITY_INSERT CaseType OFF

SELECT * FROM CaseType WHERE JurisdictionId = @JurisdictionId
    COMMIT
SET XACT_ABORT OFF