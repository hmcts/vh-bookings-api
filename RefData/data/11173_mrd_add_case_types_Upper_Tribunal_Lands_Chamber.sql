USE vhbookings
SET XACT_ABORT ON


BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.Jurisdiction WHERE Name = 'Upper Tribunal Lands Chamber')
    INSERT INTO Jurisdiction (Code, Name, CreatedDate, UpdatedDate, IsLive)
    VALUES ('Upper Tribunal Lands Chamber', 'Upper Tribunal Lands Chamber', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1)
END
GO;

BEGIN TRANSACTION

SET IDENTITY_INSERT CaseType ON
DECLARE @JurisdictionId INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Upper Tribunal Lands Chamber')
UPDATE CaseType SET JurisdictionId = @JurisdictionId, UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId IN
(
    'BLA1',
    'BLA2',
    'BLA3',
    'BLA4',
    'BLA5',
    'BLA6',
    'BLA7',
    'BLA8',
    'BLA9',
    'BLB1',
    'BLB2'
)

GO;
SET IDENTITY_INSERT CaseType OFF

SELECT * FROM CaseType WHERE ID >= 75

GO;

COMMIT
SET XACT_ABORT OFF