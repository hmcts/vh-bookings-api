SET XACT_ABORT ON
BEGIN TRANSACTION

UPDATE VhBookings.dbo.HearingVenue SET IsWorkAllocationEnabled = 1, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Teesside Combined Court Centre'
UPDATE VhBookings.dbo.HearingVenue SET IsWorkAllocationEnabled = 1, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Teesside Magistrates Court'
UPDATE VhBookings.dbo.HearingVenue SET IsWorkAllocationEnabled = 1, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Darlington County Court and Family Court'
UPDATE VhBookings.dbo.HearingVenue SET IsWorkAllocationEnabled = 1, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Darlington Magistrates Court and Family Court'

COMMIT
SET XACT_ABORT OFF