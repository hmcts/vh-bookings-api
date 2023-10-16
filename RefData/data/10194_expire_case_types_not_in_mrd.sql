SET XACT_ABORT ON
GO;

BEGIN TRANSACTION;

UPDATE VhBookings.dbo.CaseType SET ExpirationDate = '2024-01-24', UpdatedDate = CURRENT_TIMESTAMP WHERE Name IN
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
'Tribunal'
)

GO;

SELECT * FROM VhBookings.dbo.CaseType WHERE ExpirationDate = '2024-01-24'

COMMIT;
SET XACT_ABORT OFF