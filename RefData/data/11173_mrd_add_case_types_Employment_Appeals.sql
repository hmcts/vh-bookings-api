USE vhbookings
SET XACT_ABORT ON


BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.Jurisdiction WHERE Name = 'Employment Appeals Tribunal')
    INSERT INTO Jurisdiction (Code, Name, CreatedDate, UpdatedDate, IsLive)
    VALUES ('Employment Appeals Tribunal', 'Employment Appeals Tribunal', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1)
END
GO;

BEGIN TRANSACTION

SET IDENTITY_INSERT CaseType ON
DECLARE @JurisdictionId INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Employment Appeals Tribunal')
UPDATE CaseType SET JurisdictionId = @JurisdictionId, UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'BMA1';
UPDATE CaseType SET JurisdictionId = @JurisdictionId, UpdatedDate = CURRENT_TIMESTAMP  WHERE ServiceId = 'BMA2';
UPDATE CaseType SET JurisdictionId = @JurisdictionId, UpdatedDate = CURRENT_TIMESTAMP  WHERE ServiceId = 'BMA3';

GO;
SET IDENTITY_INSERT CaseType OFF

SELECT * FROM CaseType WHERE ID >= 75

GO;

COMMIT
SET XACT_ABORT OFF