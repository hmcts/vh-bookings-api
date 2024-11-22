SET XACT_ABORT ON
GO;

BEGIN TRANSACTION;

UPDATE VhBookings.dbo.CaseType SET ExpirationDate = '2024-11-29', UpdatedDate = CURRENT_TIMESTAMP WHERE Name IN 
(
    'Employment Appeal Tribunal', 
    'Upper Tribunal Administrative Appeals Chamber',
    'Upper Tribunal Lands Chamber' 
)

GO;

SELECT * FROM VhBookings.dbo.HearingVenue WHERE HearingVenue.ExpirationDate IS NOT NULL

COMMIT TRANSACTION;
SET XACT_ABORT OFF