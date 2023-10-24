USE vhbookings;
SET XACT_ABORT ON
GO


CREATE PROCEDURE #CREATE_CASE_TYPES_V10178 @CaseTypeId INT, @name varchar(max), @serviceId varchar(max)
AS
DECLARE @JurisdictionId INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Civil')
BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.CaseType WHERE Id = @CaseTypeId)
BEGIN
    INSERT INTO CaseType (Id, Name, CreatedDate, UpdatedDate, JurisdictionId, Live, ServiceId)
    VALUES (@CaseTypeId, @name, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @JurisdictionId, 1, @serviceId)
END
END
GO

BEGIN TRANSACTION

SET IDENTITY_INSERT CaseType ON
EXEC #CREATE_CASE_TYPES_V10178 56, 'Civil Enforcement', 'AAA1';
EXEC #CREATE_CASE_TYPES_V10178 57, 'Insolvency', 'AAA2';
EXEC #CREATE_CASE_TYPES_V10178 58, 'Mortgage and Landlord Possession Claims', 'AAA3';
EXEC #CREATE_CASE_TYPES_V10178 59, 'Non-money Claims', 'AAA4';
EXEC #CREATE_CASE_TYPES_V10178 60, 'Return of Goods Claims', 'AAA5';
UPDATE CaseType SET Name = 'Specified Money Claims' WHERE Name = 'Civil Money Claims';

SET IDENTITY_INSERT CaseType OFF

SELECT * FROM CaseType WHERE ID >= 56 AND ID <= 62

    COMMIT
SET XACT_ABORT OFF