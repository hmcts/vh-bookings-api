SET XACT_ABORT ON
GO;

BEGIN TRANSACTION;

UPDATE VhBookings.dbo.HearingVenue SET ExpirationDate = '2023-09-01', UpdatedDate = CURRENT_TIMESTAMP WHERE Name IN
(
'Barrow-in-Furness Magistrates Court',
'Basingstoke Magistrates'' Court',
'Birmingham Immigration and Asylum Chamber',
'Birmingham Social Security and Child Support Tribunal',
'Birmingham Youth Court',
'Bolton Magistrates Court',
'Boston Magistrates Court',
'Bournemouth Crown Court',
'Central London County Court',
'Court of Protection',
'Crewe County Court and Family Court',
'Crown Court',
'Croydon Crown Court',
'Derby Social Security and Child Support Tribunal',
'Dudley County Court and Family Court',
'Eastern Property Tribunal',
'Edinburgh Employment Appeal Tribunal',
'Edinburgh Upper Tribunal (Administrative Appeals Chamber)',
'Gateshead County Court and Family Court',
'Hastings County Court and Family Court',
'Haverfordwest Magistrates Court',
'High Wycombe County Court and Family Court',
'Horsham Magistrates Court',
'King''s Lynn Magistrates Court and Family Court',
'Laganside Courts',
'Land Registration',
'Liverpool and Knowsley Magistrates Court',
'Liverpool District Probate Registry',
'London (South) Employment Tribunal',
'London Circuit Commercial Court',
'Mental Health Tribunal',
'Middlesbrough County Court',
'Midlands Property Tribunal',
'Northern Property Tribunal',
'Nottingham Crown Court',
'Nottingham Justice Centre',
'Nottingham Social Security and Child Support Tribunal',
'Skipton Magistrates Court',
'South Tyneside Magistrates Court',
'Southend Magistrates Court',
'Staines County Court and Family Court',
'Stockport County Court and Family Court',
'War and Pension Tribunal',
'Warrington Magistrates Court',
'Warwick Combined Court',
'Wigan County Court and Family Court',
'Worthing Magistrates Court',
'Wrexham Magistrates Court',
'Residential Property Tribunal',
'Glasgow Tribunals Centre',
'Hamilton Brandon Gate',
'Inverness Employment Tribunal',
'Lancaster Court House',
'Leeds District Probate Registry',
'Newcastle upon Tyne Crown Court and Magistrates Court',
'Newport',
'Northampton Social Security and Child Support Tribunal',
'Warrington Crown Court',
'Guildford County Court and Family Court'
)

GO;

SELECT * FROM VhBookings.dbo.HearingVenue WHERE HearingVenue.ExpirationDate IS NOT NULL

COMMIT TRANSACTION ;
SET XACT_ABORT OFF