USE vhbookings
SET XACT_ABORT ON


BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.Jurisdiction WHERE Name = 'Upper Tribunal Immigration and Asylum Chamber')
    INSERT INTO Jurisdiction (Code, Name, CreatedDate, UpdatedDate, IsLive)
    VALUES ('Upper Tribunal Immigration and Asylum Chamber', 'Upper Tribunal Immigration and Asylum Chamber', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1)
END
GO;

BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.Jurisdiction WHERE Name = 'Upper Tribunal Administrative Appeals Chamber')
    INSERT INTO Jurisdiction (Code, Name, CreatedDate, UpdatedDate, IsLive)
    VALUES ('Upper Tribunal Administrative Appeals Chamber', 'Upper Tribunal Administrative Appeals Chamber', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1)
END
GO;

BEGIN TRANSACTION

SET IDENTITY_INSERT CaseType ON
DECLARE @JurisdictionId INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Upper Tribunal Immigration and Asylum Chamber')
UPDATE CaseType SET JurisdictionId = @JurisdictionId, UpdatedDate = CURRENT_TIMESTAMP WHERE serviceId = 'BIA1'
DECLARE @JurisdictionId2 INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Upper Tribunal Administrative Appeals Chamber')
UPDATE CaseType SET JurisdictionId = @JurisdictionId2, UpdatedDate = CURRENT_TIMESTAMP WHERE serviceId IN
(
    'BKA4',
    'BKA5',
    'BKA6',
    'BKA7',
    'BKA8',
    'BKA9',
    'BKB1',
    'BKB2',
    'BKB3',
    'BKB4',
    'BKB5',
    'BKB6',
    'BKB7',
    'BKB8',
    'BKB9',
    'BKC1',
    'BKC2',
    'BKC3',
    'BKC4',
    'BKC5',
    'BKC6'
)


GO;
SET IDENTITY_INSERT CaseType OFF

SELECT * FROM CaseType WHERE ID >= 75

GO;

COMMIT
SET XACT_ABORT OFF