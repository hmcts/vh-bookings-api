SET XACT_ABORT ON;
BEGIN TRANSACTION;

BEGIN
    IF NOT EXISTS(SELECT * FROM HearingVenue WHERE Name = 'TempMigrationVenue')
        BEGIN
            Insert Into HearingVenue (Name, Id, CreatedDate, UpdatedDate, VenueCode, IsScottish, IsWorkAllocationEnabled)  VALUES ('TempMigrationVenue', 9999, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 'xxxxxx', 0, 0)
        END
END
GO;


-- update all hearings where venueName = <old name> to 'TempMigrationVenue'
-- update venue details
-- update all hearings where venueName = 'TempMigrationVenue' to <new name>
CREATE PROC #HearingVenue_UpdateVenueCodeAndName @oldVenueName nvarchar(max), @venueCode varchar(450), @newVenueName nvarchar(max)
As
BEGIN
    IF EXISTS (SELECT * FROM dbo.HearingVenue WHERE Name = TRIM(@oldVenueName))
        BEGIN
            -- insert into the temp table where ids do not already exist
            Print ('FOUND venue with the name: ' + @oldVenueName);
            Update Hearing SET HearingVenueName = 'TempMigrationVenue' WHERE HearingVenueName = @oldVenueName;
            UPDATE HearingVenue SET Name = TRIM(@newVenueName), VenueCode = TRIM(@venueCode), UpdatedDate = CURRENT_TIMESTAMP WHERE Name = TRIM(@oldVenueName);
            Update Hearing SET HearingVenueName = @newVenueName WHERE HearingVenueName = 'TempMigrationVenue';
        END
    ELSE
        BEGIN
            Print ('WARNING! Could not find venue with the name: ' + @oldVenueName);
        END
END
GO;

SELECT * FROM HearingVenue;
GO;

EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Ayr', @venueCode = '206150', @newVenueName='Ayr Social Security and Child Support Tribunal';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Bath Law Courts (Civil and Family)', @venueCode = '411234', @newVenueName='Bath Magistrates Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Birkenhead County Court', @venueCode = '444097', @newVenueName='Birkenhead County Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Birmingham Employment Tribunal', @venueCode = '877347', @newVenueName='Centre City Tower';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Blackburn Family Court', @venueCode = '150431', @newVenueName='Blackburn County Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Blackpool Family Court', @venueCode = '214320', @newVenueName='Blackpool County Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Bodmin Law Courts', @venueCode = '271813', @newVenueName='Bodmin County Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Bolton Crown Court', @venueCode = '447533', @newVenueName='Bolton Combined Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Bridlington Magistrates Court and Hearing Centre', @venueCode = '759555', @newVenueName='Bridlington Magistrates Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Brighton County Court', @venueCode = '478896', @newVenueName='Brighton County and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Brighton Social Security and Child Support Tribunal', @venueCode = '296806', @newVenueName='Brighton Tribunal Hearing Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Bristol Magistrates Court and Tribunals Hearing Centre', @venueCode = '781155', @newVenueName='Bristol Magistrates Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Carmarthen County Court and Family Court', @venueCode = '101959', @newVenueName='Carmarthen County Court and Tribunal Hearing Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Central Family Court', @venueCode = '356855', @newVenueName='Central Family Court (First Avenue House)';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Chelmsford Justice Centre', @venueCode = '816875', @newVenueName='Chelmsford County and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Chesterfield Justice Centre', @venueCode = '652852', @newVenueName='Chesterfield Magistrates';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Crewe Magistrates Court', @venueCode = '566296', @newVenueName='Crewe (South Cheshire) Magistrates Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Derby Magistrates Court', @venueCode = '484482', @newVenueName='Derby Magistrates';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Dundee Tribunal Hearing Centre', @venueCode = '367564', @newVenueName='Dundee Tribunal Hearing Centre - Endeavour House';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Durham County Court and Family Court', @venueCode = '491107', @newVenueName='Durham Justice Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='East Berkshire Magistrates Court', @venueCode = '345045', @newVenueName='East Berkshire Magistrates Court, Maidenhead';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Exeter Law Courts', @venueCode = '735217', @newVenueName='Exeter Combined Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Hull and Holderness Magistrates Court and Hearing Centre', @venueCode = '362420', @newVenueName='Hull Magistrates Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Lavender Hill Magistrates Court', @venueCode = '536548', @newVenueName='Lavender Hill Magistrates Court (Formerly South Western Magistrates Court)';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Leicester County Court and Family Court', @venueCode = '223503', @newVenueName='Leicester County Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Leyland Family Hearing Centre', @venueCode = '415903', @newVenueName='Leyland Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Luton and South Bedfordshire Magistrates Court', @venueCode = '252292', @newVenueName='Luton and South Bedfordshire Magistrates Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Maidstone Magistrates Court', @venueCode = '782795', @newVenueName='Maidstone Magistrates Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Manchester Civil and Family Justice Centre', @venueCode = '701411', @newVenueName='Manchester County and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Manchester Employment Tribunal', @venueCode = '301017', @newVenueName='Manchester Tribunal Hearing Centre - Alexandra House';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Manchester Tribunal Hearing Centre', @venueCode = '512401', @newVenueName='Manchester Tribunal Hearing Centre - Piccadilly Exchange';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Medway Magistrates Court and Family Court', @venueCode = '771467', @newVenueName='Medway Magistrates Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Mold Justice Centre (Mold Law Courts)', @venueCode = '211138', @newVenueName='Mold Justice Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Newton Aycliffe Magistrates Court', @venueCode = '659436', @newVenueName='Newton Aycliffe Magistrates Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Northampton Crown, County and Family Court', @venueCode = '195489', @newVenueName='Northampton Crown Court, County Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Oxford Magistrates Court', @venueCode = '732661', @newVenueName='Oxford and Southern Oxfordshire Magistrates Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Plymouth (St Catherine''s House)', @venueCode = '235590', @newVenueName='Plymouth As St Catherine''s House';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Preston Crown Court and Family Court (Sessions House)', @venueCode = '102476', @newVenueName='Preston Crown Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Reedley Family Hearing Centre', @venueCode = '739294', @newVenueName='Reedley Magistrates Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Sheffield Designated Family Court', @venueCode = '778638', @newVenueName='Sheffield Family Hearing Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Shrewsbury Crown Court', @venueCode = '259170', @newVenueName='Shrewsbury Justice Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Skipton County Court and Family Court', @venueCode = '318324', @newVenueName='Skipton Magistrates and County Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='South Tyneside County Court and Family Court', @venueCode = '563156', @newVenueName='South Tyneside Magistrates Court and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Southampton Hearing Centre', @venueCode = '43104', @newVenueName='Southampton Combined Court Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Southend Crown Court', @venueCode = '781139', @newVenueName='Southend Combined - Crown, Mags, County and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Southern Property Tribunal', @venueCode = '710684', @newVenueName='Southern House';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Stirling Wallace House', @venueCode = '198561', @newVenueName='Stirling Tribunal Hearing Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Stockport Magistrates Court', @venueCode = '560788', @newVenueName='Stockport Magistrates Court and Famiy Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Swansea Civil Justice Centre', @venueCode = '234946', @newVenueName='Swansea Civil and Family Justice Centre';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Taunton Combined Court', @venueCode = '382409', @newVenueName='Taunton Crown, County and Family Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='West Hampshire (Southampton) Magistrates Court', @venueCode = '330480', @newVenueName='West Hampshire Magistrates Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Wolverhampton Social Security and Child Support Tribunal', @venueCode = '788436', @newVenueName='Wolverhampton Ast - Norwich Union House, Wolverhampton';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Worcester Justice Centre', @venueCode = '703200', @newVenueName='Worcester Magistrates Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Worthing County Court and Family Court', @venueCode = '493880', @newVenueName='Worthing Magistrates and County Court';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Wrexham County and Family Court', @venueCode = '637145', @newVenueName='Wrexham Law Courts';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='King''s Lynn Crown Court', @venueCode = '671879', @newVenueName='King''s Lynn Crown Court (& Magistrates)';
EXEC #HearingVenue_UpdateVenueCodeAndName @oldVenueName='Hereford Crown Court', @venueCode = '236611', @newVenueName='Hereford Crown Court';
GO

Delete from HearingVenue Where Name = 'TempMigrationVenue' AND Id = 9999 and VenueCode = 'xxxxxx';
GO

SELECT * FROM HearingVenue;
GO;

DROP PROC #HearingVenue_UpdateVenueCodeAndName;
GO;

COMMIT;
SET XACT_ABORT OFF