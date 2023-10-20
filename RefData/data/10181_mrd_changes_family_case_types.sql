USE vhbookings;
SET XACT_ABORT ON
GO

CREATE PROCEDURE #CREATE_CASE_TYPES_V10181 @CaseTypeId INT, @name varchar(max), @serviceId varchar(max), @JurisdictionId INT
AS
BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.CaseType WHERE Id = @CaseTypeId)
    BEGIN
        INSERT INTO CaseType (Id, Name, CreatedDate, UpdatedDate, JurisdictionId, Live, ServiceId)
        VALUES (@CaseTypeId, @name, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @JurisdictionId, 1, @serviceId)
    END
END
GO

BEGIN TRANSACTION

DECLARE @JurisdictionId INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Family')

UPDATE CaseType SET Name = 'Family Public Law', JurisdictionId = @JurisdictionId, ServiceId = 'ABA3' WHERE Name = 'Public Law - Care'
UPDATE CaseType SET Name = 'Family Private Law', JurisdictionId = @JurisdictionId, ServiceId = 'ABA5' WHERE Name = 'Private Law'

SET IDENTITY_INSERT CaseType ON
EXEC #CREATE_CASE_TYPES_V10181 61, 'Probate', 'ABA6', @JurisdictionId;
EXEC #CREATE_CASE_TYPES_V10181 62, 'Court of Protections', 'ABA7', @JurisdictionId;
EXEC #CREATE_CASE_TYPES_V10181 63, 'REMO', 'ABA8', @JurisdictionId;
EXEC #CREATE_CASE_TYPES_V10181 64, 'Maintenance Enforcement', 'ABA9', @JurisdictionId;
SET IDENTITY_INSERT CaseType OFF
            
SELECT * FROM CaseType WHERE ServiceId = 'ABA3' OR ServiceId <= 'ABA5'
SELECT * FROM CaseType WHERE ID >= 61 AND ID <= 64

COMMIT
SET XACT_ABORT OFF