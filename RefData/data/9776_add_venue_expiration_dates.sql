SET XACT_ABORT ON
GO;

BEGIN TRANSACTION;

UPDATE VhBookings.dbo.HearingVenue SET ExpirationDate = '2023-09-01' WHERE Name = 'Crown Court'
UPDATE VhBookings.dbo.HearingVenue SET ExpirationDate = '2023-09-01' WHERE Name = 'Eastern Property Tribunal'
UPDATE VhBookings.dbo.HearingVenue SET ExpirationDate = '2023-09-01' WHERE Name = 'Edinburgh Employment Appeal Tribunal'
UPDATE VhBookings.dbo.HearingVenue SET ExpirationDate = '2023-09-01' WHERE Name = 'Middlesbrough County Court'
UPDATE VhBookings.dbo.HearingVenue SET ExpirationDate = '2023-09-01' WHERE Name = 'Midlands Property Tribunal'
UPDATE VhBookings.dbo.HearingVenue SET ExpirationDate = '2023-09-01' WHERE Name = 'Northern Property Tribunal'

GO;

SELECT * FROM VhBookings.dbo.HearingVenue WHERE HearingVenue.ExpirationDate IS NOT NULL

COMMIT TRANSACTION ;
SET XACT_ABORT OFF