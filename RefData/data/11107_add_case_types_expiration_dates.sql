SET XACT_ABORT ON
GO;

BEGIN TRANSACTION;

UPDATE VhBookings.dbo.CaseType SET ExpirationDate = '2024-11-29', UpdatedDate = CURRENT_TIMESTAMP WHERE Name IN 
(
    'Business Lease Renewal',
    'Children Act',
    'Civil',
    'Family',
    'Family Law Act',
    'GRC - Alternative Business Structures',
    'GRC - Claims Management Services',
    'GRC - Consumer Credit',
    'GRC - EJ',
    'GRC - Local Government Standards',
    'GRC - Professional Regulations',
    'Housing & Planning Act',
    'Housing Act',
    'Leasehold Enfranchisement',
    'Leasehold Management',
    'Park Homes',
    'Placement',
    'Rents',
    'Right to buy',
    'Tenant Fees',
    'Tribunal',
    'Employment Appeal Tribunal',
    'Upper Tribunal Administrative Appeals Chamber',
    'Upper Tribunal Lands Chamber'
)

GO;

SELECT * FROM VhBookings.dbo.HearingVenue WHERE HearingVenue.ExpirationDate IS NOT NULL

COMMIT TRANSACTION;
SET XACT_ABORT OFF