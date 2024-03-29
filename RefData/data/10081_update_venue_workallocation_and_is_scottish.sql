SET XACT_ABORT ON
BEGIN TRANSACTION

Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 366559, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Atlantic Quay Glasgow';
Update VhBookings.dbo.HearingVenue SET IsScottish = 0, IsWorkAllocationEnabled = 1, VenueCode = 999976, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Conwy';
Update VhBookings.dbo.HearingVenue SET IsScottish = 0, IsWorkAllocationEnabled = 0, VenueCode = 288691, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Darlington County Court and Family Court';
Update VhBookings.dbo.HearingVenue SET IsScottish = 0, IsWorkAllocationEnabled = 0, VenueCode = 476820, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Darlington Magistrates Court and Family Court';
Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 999993, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Galashiels';
Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 723075, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Hamilton Social Security and Child Support Tribunal';
Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 107378, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Inverness Social Security and Child Support Tribunal';
Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 999991, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Kilmarnock';
Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 999990, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Kirkcaldy';
Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 999989, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Kirkwall';
Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 999988, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Lerwick';
Update VhBookings.dbo.HearingVenue SET IsScottish = 1, IsWorkAllocationEnabled = 0, VenueCode = 999984, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = 'Stranraer';

COMMIT
SET XACT_ABORT OFF