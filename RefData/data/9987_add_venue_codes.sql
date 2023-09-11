USE VhBookings;

SET XACT_ABORT ON
GO;
CREATE OR ALTER PROC #HearingVenue_CreateIfNotExist @id int, @venueName nvarchar(max), @venueCode varchar(450), @isScottish int,  @isWorkAllocationEnabled int
As
BEGIN
    IF NOT EXISTS(SELECT * FROM HearingVenue WHERE Name = @venueName)
        BEGIN
            SET IDENTITY_INSERT dbo.HearingType ON
            Insert Into HearingVenue (Name, Id, CreatedDate, UpdatedDate, VenueCode, IsScottish, IsWorkAllocationEnabled)  VALUES (@venueName, @id, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @venueCode, @isScottish, @isWorkAllocationEnabled)
    
            SET IDENTITY_INSERT dbo.HearingType OFF
        END
END
GO;

CREATE OR ALTER PROC #HearingVenue_UpdateVenueCode @venueName nvarchar(max), @venueCode varchar(450)
As
BEGIN
    IF EXISTS (SELECT * FROM dbo.HearingVenue WHERE Name = @venueName)
        BEGIN
            UPDATE HearingVenue
            SET VenueCode = @venueCode,
                UpdatedDate = CURRENT_TIMESTAMP
            WHERE Name = @venueName;
        END
    ELSE
        BEGIN
            Print ('WARNING! Could not find venue with the name: ' + @venueName);
        END
end
GO;

BEGIN TRANSACTION;

-- New venues to be added
-- change to an insert

EXEC #HearingVenue_CreateIfNotExist @id = 465, @venueName = 'Collection Enforcement Centre', @venueCode = '534560', @isScottish = 0, @isWorkAllocationEnabled = 1;
EXEC #HearingVenue_CreateIfNotExist @id = 466, @venueName = 'Council Chamber-Tunbridge Wells Town Hall', @venueCode = '213971', @isScottish = 0, @isWorkAllocationEnabled = 1;
EXEC #HearingVenue_CreateIfNotExist @id = 467, @venueName = 'National Business Centre, Salford', @venueCode = '192280', @isScottish = 0, @isWorkAllocationEnabled = 1;
EXEC #HearingVenue_CreateIfNotExist @id = 468, @venueName = 'Crown House (Loughborough Offices)', @venueCode = '420587', @isScottish = 0, @isWorkAllocationEnabled = 1;
EXEC #HearingVenue_CreateIfNotExist @id = 469, @venueName = 'Cwmbran Offices (Gwent House)', @venueCode = '339076', @isScottish = 0, @isWorkAllocationEnabled = 1;
EXEC #HearingVenue_CreateIfNotExist @id = 470, @venueName = 'Dumfries (1)', @venueCode = '999997', @isScottish = 1, @isWorkAllocationEnabled = 0;
EXEC #HearingVenue_CreateIfNotExist @id = 471, @venueName = 'Dunfermline', @venueCode = '999995', @isScottish = 1, @isWorkAllocationEnabled = 0;
EXEC #HearingVenue_CreateIfNotExist @id = 472, @venueName = 'Wick', @venueCode = '999983', @isScottish = 1, @isWorkAllocationEnabled = 0;
EXEC #HearingVenue_CreateIfNotExist @id = 473, @venueName = 'Conwy', @venueCode = '999976', @isScottish = 0, @isWorkAllocationEnabled = 0;
EXEC #HearingVenue_CreateIfNotExist @id = 474, @venueName = 'Dolgellau', @venueCode = '999978', @isScottish = 0, @isWorkAllocationEnabled = 0;

-- Fix typoes
UPDATE HearingVenue SET Name = 'Sefton Magistrates Court', VenueCode = '395973', UpdatedDate = CURRENT_TIMESTAMP WHERE Name LIKE 'Sefton Magistrates Court%'

EXEC #HearingVenue_UpdateVenueCode @venueName = 'Aberdeen Tribunal Hearing Centre', @venueCode = '219164';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Aberystwyth Justice Centre', @venueCode = '827534';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Aldershot Justice Centre', @venueCode = '450049';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Amersham Law Courts', @venueCode = '271588';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Ashford Tribunal Hearing Centre', @venueCode = '239985';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Aylesbury Crown Court', @venueCode = '817181';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Barkingside Magistrates Court', @venueCode = '218723';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Barnet Civil and Family Courts Centre', @venueCode = '229786';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Barnsley Law Courts', @venueCode = '574546';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Barnstaple Magistrates, County and Family Court', @venueCode = '774335';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Barrow-in-Furness County Court and Family Court', @venueCode = '761518';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Basildon Combined Court', @venueCode = '694840';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Basildon Magistrates Court and Family Court', @venueCode = '538351';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Basingstoke County Court and Family Court', @venueCode = '457273';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bedford County Court and Family Court', @venueCode = '446255';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Berwick upon Tweed Magistrates Court', @venueCode = '500233';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Beverley Magistrates Court and Hearing Centre', @venueCode = '359723';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bexley Magistrates Court', @venueCode = '381649';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bexleyheath Social Security and Child Support Tribunal', @venueCode = '29955';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Birmingham Civil and Family Justice Centre', @venueCode = '231596';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Birmingham Crown Court', @venueCode = '482914';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Birmingham Magistrates Court', @venueCode = '784730';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Blackburn Magistrates Court', @venueCode = '215156';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Blackpool Magistrates and Civil Court', @venueCode = '336348';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Blackwood Civil and Family Court', @venueCode = '304576';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bolton Social Security and Child Support Tribunal', @venueCode = '210228';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Boston County Court and Family Court', @venueCode = '117667';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bournemouth Combined Court', @venueCode = '101633';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bradford and Keighley Magistrates Court and Family Court', @venueCode = '580554';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bradford Combined Court Centre', @venueCode = '88516';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bradford Tribunal Hearing Centre', @venueCode = '698118';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Brentford County and Family Court', @venueCode = '36791';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Brighton Hearing Centre', @venueCode = '298390';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Brighton Magistrates Court', @venueCode = '417418';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bristol Civil and Family Justice Centre', @venueCode = '819890';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bristol Crown Court', @venueCode = '477819';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bromley County Court and Family Court', @venueCode = '29656';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bromley Magistrates Court', @venueCode = '784131';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Burnley Combined Court Centre', @venueCode = '448747';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Burnley Magistrates Court', @venueCode = '754614';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bury St Edmunds County Court and Family Court', @venueCode = '257431';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Caernarfon Justice Centre', @venueCode = '366572';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cambridge County Court and Family Court', @venueCode = '650344';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cambridge Crown Court', @venueCode = '507931';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cambridge Magistrates Court', @venueCode = '602674';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cannock Magistrates Court', @venueCode = '272966';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Canterbury Combined Court Centre', @venueCode = '259679';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Canterbury Magistrates Court', @venueCode = '411663';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cardiff Civil and Family Justice Centre', @venueCode = '234850';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cardiff Crown Court', @venueCode = '302630';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cardiff Magistrates Court', @venueCode = '779109';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cardiff Social Security and Child Support Tribunal', @venueCode = '372653';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Carlisle Combined Court', @venueCode = '45106';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Carlisle Magistrates Court', @venueCode = '243126';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Central Criminal Court', @venueCode = '312962';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Central London Employment Tribunal', @venueCode = '21153';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Chelmsford Crown Court', @venueCode = '428073';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Chelmsford Magistrates Court and Family Court', @venueCode = '624810';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cheltenham Magistrates Court', @venueCode = '711798';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Chester Civil and Family Justice Centre', @venueCode = '226511';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Chester Crown Court', @venueCode = '468040';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Chester Magistrates Court', @venueCode = '443014';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Clerkenwell and Shoreditch County Court and Family Court', @venueCode = '739514';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Colchester Magistrates Court and Family Court', @venueCode = '407566';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Coventry Combined Court Centre', @venueCode = '497679';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Coventry Magistrates Court', @venueCode = '787030';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Crawley Magistrates Court', @venueCode = '248100';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Croydon County Court and Family Court', @venueCode = '407494';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Croydon Magistrates Court', @venueCode = '747662';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Cwmbran Magistrates Court', @venueCode = '296111';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Darlington County Court and Family Court', @venueCode = '288691';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Darlington Magistrates Court and Family Court', @venueCode = '476820';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Dartford County Court and Family Court', @venueCode = '194172';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Derby Combined Court Centre', @venueCode = '201339';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Doncaster Justice Centre North', @venueCode = '640119';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Doncaster Justice Centre South', @venueCode = '45900';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Dudley Magistrates Court', @venueCode = '758998';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Durham Crown Court', @venueCode = '386393';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Ealing Magistrates Court', @venueCode = '597501';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Slough County Court And Family Court', @venueCode = '224403';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'East London Family Court', @venueCode = '898213';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'East London Tribunal Hearing Centre', @venueCode = '816964';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Edinburgh Employment Tribunal', @venueCode = '368308';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Edinburgh Social Security and Child Support Tribunal', @venueCode = '604040';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Edmonton County Court and Family Court', @venueCode = '25463';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Enfield Social Security and Child Support Tribunal', @venueCode = '208506';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Field House Tribunal Hearing Centre', @venueCode = '652070';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Folkestone Magistrates Court', @venueCode = '307399';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Gateshead Magistrates Court and Family Court', @venueCode = '427519';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Gloucester Crown Court', @venueCode = '100539';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Gloucester and Cheltenham County and Family Court', @venueCode = '198592';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Great Yarmouth Magistrates Court and Family Court', @venueCode = '609320';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Great Grimsby Combined Court Centre', @venueCode = '478126';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Grimsby Magistrates Court and Family Court', @venueCode = '641199';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Guildford Crown Court', @venueCode = '468679';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Guildford Magistrates Court and Family Court', @venueCode = '568484';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Harmondsworth Tribunal Hearing Centre', @venueCode = '28837';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Harrogate Justice Centre', @venueCode = '505683';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Harrow Crown Court', @venueCode = '29096';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hastings Magistrates Court', @venueCode = '784691';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hatfield Magistrates Court', @venueCode = '816879';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hatton Cross Tribunal Hearing Centre', @venueCode = '386417';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Havant Justice Centre', @venueCode = '741758';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Haverfordwest County Court and Family Court', @venueCode = '700596';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hendon Magistrates Court', @venueCode = '745389';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hereford Crown Court', @venueCode = '236611';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hertford County Court and Family Court', @venueCode = '256913';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'High Wycombe Magistrates Court and Family Court', @venueCode = '437303';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Highbury Corner Magistrates Court', @venueCode = '416695';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Horsham County Court and Family Court', @venueCode = '317442';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hove Trial Centre', @venueCode = '198396';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Huddersfield County Court and Family Court', @venueCode = '197852';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Huntingdon Law Courts', @venueCode = '648275';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Inner London Crown Court', @venueCode = '337856';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Inverness Justice Centre', @venueCode = '900001';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Ipswich County Court and Family Hearing Centre', @venueCode = '471349';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Ipswich Crown Court', @venueCode = '357989';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Ipswich Magistrates Court', @venueCode = '287515';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Isle of Wight Combined Court', @venueCode = '416742';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Isleworth Crown Court', @venueCode = '718075';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Kidderminster Magistrates Court', @venueCode = '767032';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'King''s Lynn Crown Court (& Magistrates)', @venueCode = '671879';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Kingston upon Thames County Court and Family Court', @venueCode = '13660';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Kingston upon Thames Crown Court', @venueCode = '699560';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Kingston-upon-Hull Combined Court Centre', @venueCode = '195520';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Kirklees (Huddersfield) Magistrates Court and Family Court', @venueCode = '320113';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Leeds Combined Court Centre', @venueCode = '455174';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Leeds Employment Tribunal', @venueCode = '36313';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Leeds Magistrates Court and Family Court', @venueCode = '569737';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Leeds Social Security and Child Support Tribunal', @venueCode = '495952';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Leicester Crown Court', @venueCode = '425094';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Leicester Magistrates Court', @venueCode = '417439';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Leicester Tribunal Hearing Centre', @venueCode = '500064';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Lewes Combined Court Centre', @venueCode = '101183';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Lincoln County Court and Family Court', @venueCode = '195465';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Lincoln Crown Court', @venueCode = '318389';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Lincoln Magistrates Court', @venueCode = '534157';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Liverpool Civil and Family Court', @venueCode = '345663';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Liverpool Crown Court', @venueCode = '448077';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Liverpool Social Security and Child Support Tribunal', @venueCode = '196538';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Llandrindod Wells Magistrates and Family Court', @venueCode = '860090';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Llandudno Magistrates Court', @venueCode = '361595';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Llanelli Law Courts', @venueCode = '390932';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Loughborough Court', @venueCode = '815728';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Luton Crown Court', @venueCode = '486853';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Luton Justice Centre', @venueCode = '365554';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Maidstone Combined Court Centre', @venueCode = '465872';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Manchester Crown Court (Crown Square)', @venueCode = '144641';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Manchester Crown Court (Minshull St)', @venueCode = '326944';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Manchester Magistrates Court', @venueCode = '783803';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Mansfield Magistrates and County Court', @venueCode = '455368';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Margate Magistrates Court', @venueCode = '659591';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Mayor''s and City of London Court', @venueCode = '674229';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Medway County Court and Family Court', @venueCode = '487294';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Merthyr Tydfil Combined Court Centre', @venueCode = '448345';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Mid and South East Northumberland Law Courts', @venueCode = '572158';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Milton Keynes County Court and Family Court', @venueCode = '815997';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Milton Keynes Magistrates Court and Family Court', @venueCode = '497356';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newcastle Civil & Family Courts and Tribunals Centre', @venueCode = '366796';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newcastle District Probate Registry', @venueCode = '417628';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newcastle Moot Hall', @venueCode = '373939';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newport (South Wales) County Court and Family Court', @venueCode = '217250';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newport (South Wales) Crown Court', @venueCode = '200518';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newport (South Wales) Magistrates Court', @venueCode = '324413';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newton Abbot Magistrates Court', @venueCode = '270253';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'North Shields County Court and Family Court', @venueCode = '562808';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'North Somerset Magistrates Court', @venueCode = '545334';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'North Staffordshire Justice Centre', @venueCode = '370964';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'North Tyneside Magistrates Court', @venueCode = '443257';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Northampton Magistrates Court', @venueCode = '602948';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Norwich Combined Court Centre', @venueCode = '471332';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Norwich Magistrates Court and Family Court', @venueCode = '777942';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Norwich Social Security and Child Support Tribunal', @venueCode = '631555';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Nottingham County Court and Family Court', @venueCode = '424213';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Nottingham Magistrates Court', @venueCode = '618632';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Oxford Combined Court Centre', @venueCode = '371016';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Peterborough Combined Court Centre', @venueCode = '471569';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Peterborough Magistrates Court', @venueCode = '583034';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Peterlee Magistrates Court', @venueCode = '278871';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Plymouth Combined Court', @venueCode = '339463';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Plymouth Magistrates Court', @venueCode = '764728';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Pontypridd County Court and Family Court', @venueCode = '232298';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Poole Magistrates Court', @venueCode = '490237';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Port Talbot Justice Centre', @venueCode = '846055';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Portsmouth Combined Court Centre', @venueCode = '460592';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Portsmouth Magistrates Court', @venueCode = '379247';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Prestatyn Justice Centre', @venueCode = '379656';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Preston Combined Court Centre', @venueCode = '232580';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Preston Magistrates Court', @venueCode = '317334';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Reading County Court and Family Court', @venueCode = '185657';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Reading Crown Court', @venueCode = '256379';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Reading Magistrates Court and Family Court', @venueCode = '245287';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Reading Tribunal Hearing Centre', @venueCode = '432294';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Redditch Magistrates Court', @venueCode = '634542';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Rolls Building', @venueCode = '837247';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Romford County Court and Family Court', @venueCode = '400947';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Romford Magistrates Court (formerly Havering Magistrates'' Court)', @venueCode = '239171';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Royal Courts of Justice', @venueCode = '20262';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Salisbury Law Courts', @venueCode = '817113';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Scarborough Justice Centre', @venueCode = '744412';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Sefton Magistrates Court', @venueCode = '395973';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Sevenoaks Magistrates Court and Family Court', @venueCode = '362529';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Sheffield Combined Court Centre', @venueCode = '232607';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Sheffield Magistrates Court', @venueCode = '720624';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Snaresbrook Crown Court', @venueCode = '127994';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Southend County Court and Family Court', @venueCode = '166953';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Southwark Crown Court', @venueCode = '337959';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'St Albans Crown Court', @venueCode = '198303';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'St Albans Magistrates Court', @venueCode = '625697';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'St Helens County Court and Family Court', @venueCode = '563906';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Stafford Combined Court Centre', @venueCode = '195472';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Staines Magistrates Court and Family Court', @venueCode = '298828';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Stevenage Magistrates Court', @venueCode = '284455';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Stoke-on-Trent Combined Court', @venueCode = '195496';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Sunderland County, Family, Magistrates and Tribunal Hearings', @venueCode = '517400';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Sutton Social Security and Child Support Tribunal', @venueCode = '37792';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Swansea Crown Court', @venueCode = '279152';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Swansea Magistrates Court', @venueCode = '465986';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Swindon Combined Court', @venueCode = '438850';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Swindon Magistrates Court', @venueCode = '314074';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Tameside Magistrates Court', @venueCode = '564502';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Taunton Magistrates Court, Tribunals and Family Hearing Centre', @venueCode = '506742';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Taylor House Tribunal Hearing Centre', @venueCode = '765324';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Teesside Combined Court Centre', @venueCode = '195537';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Teesside Magistrates Court', @venueCode = '449358';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Telford County Court and Family Court', @venueCode = '497583';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Telford Magistrates Court', @venueCode = '292771';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Thames Magistrates Court', @venueCode = '664444';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Torquay and Newton Abbot County Court and Family Court', @venueCode = '235617';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Truro Combined Court', @venueCode = '475776';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Truro Magistrates Court', @venueCode = '384843';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Uxbridge County Court and Family Court', @venueCode = '621184';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Uxbridge Magistrates Court', @venueCode = '264828';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wakefield Civil and Family Justice Centre', @venueCode = '852649';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Walsall County and Family Court', @venueCode = '177463';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Walsall Magistrates Court', @venueCode = '326813';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wandsworth County Court and Family Court', @venueCode = '268374';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Watford County Court and Family Court', @venueCode = '403751';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Watford Tribunal Hearing Centre', @venueCode = '685391';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wellingborough Magistrates Court', @venueCode = '535735';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Welshpool Magistrates Court', @venueCode = '103147';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'West Cumbria Courthouse', @venueCode = '209396';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'West London Family Court', @venueCode = '373584';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Westminster Magistrates Court', @venueCode = '839746';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Weymouth Combined Court', @venueCode = '624161';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wigan and Leigh Magistrates Court', @venueCode = '245068';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Willesden County Court and Family Court', @venueCode = '228015';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Willesden Magistrates Court', @venueCode = '541183';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wimbledon Magistrates Court', @venueCode = '549957';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Winchester Combined Court Centre', @venueCode = '886493';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wirral Magistrates Court', @venueCode = '360566';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wolverhampton Combined Court Centre', @venueCode = '41047';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wolverhampton Magistrates Court', @venueCode = '590621';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wood Green Crown Court', @venueCode = '403689';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Woolwich Crown Court', @venueCode = '353988';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Worcester Combined Court', @venueCode = '102050';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Yarl''s Wood Immigration and Asylum Hearing Centre', @venueCode = '649000';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Yeovil County, Family and Magistrates Court', @venueCode = '315404';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'York County Court and Family Court', @venueCode = '107581';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'York Crown Court', @venueCode = '67542';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'York Magistrates Court and Family Court', @venueCode = '783561';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'City of London Magistrates Court', @venueCode = '375859';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Lancaster Crown Court', @venueCode = '102469';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'London Property Tribunal', @venueCode = '817549';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Croydon Employment Tribunal', @venueCode = '227942';
GO;


-- Update Unknowns as of 13 July 2023
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Ardenleigh - Birmingham', @venueCode = 'unknown_6e7d818c8d6843ebb9e4acdfb3076cc3';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Barrow-in-Furness Magistrates Court', @venueCode = 'unknown_3f0b0ed33151455bb4e8ef9f30cdc3b9';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Basingstoke Magistrates'' Court', @venueCode = 'unknown_a98b2ba37b8a46a59b06040c7528eb69';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Birmingham Immigration and Asylum Chamber', @venueCode = 'unknown_415684cea1744477ba764fcdd6b9a422';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Birmingham Social Security and Child Support Tribunal', @venueCode = 'unknown_0e6bd02245f44136976d757c6d9177ae';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Birmingham Youth Court', @venueCode = 'unknown_fb18f777389a41f5b86dc6287b1119f2';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bolton Magistrates Court', @venueCode = 'unknown_d5ac8389526c44a9a803fbb2f0acf537';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Boston Magistrates Court', @venueCode = 'unknown_d28f614eb0ff4f559511d4b07265be72';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Bournemouth Crown Court', @venueCode = 'unknown_99745ecac1a74b6c99150413718c8293';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Central London County Court', @venueCode = 'unknown_60a57f34e53e41d3b16b347301684bde';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Court of Protection', @venueCode = 'unknown_e5a8c65431b34a688a47fe469489e452';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Crewe County Court and Family Court', @venueCode = 'unknown_d827a1598e444cbfabe3b44055385d17';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Crown Court', @venueCode = 'unknown_72c84a0e93204a548c2bdcd14ac3efbf';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Croydon Crown Court', @venueCode = 'unknown_b27812025e9c4fb6a1f16c7421cdd598';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Derby Social Security and Child Support Tribunal', @venueCode = 'unknown_d96e46464ea64f37ab3c23045ded1218';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Dudley County Court and Family Court', @venueCode = 'unknown_5595c860c16b46a8be6397aa78219e60';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Eastern Property Tribunal', @venueCode = 'unknown_684b2ee2211a4d32be8800f98e5db280';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Edinburgh Employment Appeal Tribunal', @venueCode = 'unknown_f78dfdff9b9445aba783e5084ee1ccdf';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Edinburgh Upper Tribunal (Administrative Appeals Chamber)', @venueCode = 'unknown_2505a5d2a87e4b628d7dca0b0d622118';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Gateshead County Court and Family Court', @venueCode = 'unknown_2a8979289e994598b3e67492a7dcd42b';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Glasgow Tribunals Centre', @venueCode = 'unknown_bbce016306d040f48278c6770bc4ee41';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Goodmayes Hospital - Essex', @venueCode = 'unknown_a8f8324fd3f94db9a44ae1f278a7e548';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Guildford County Court and Family Court', @venueCode = 'unknown_1cb37c8ca5c644ee9a74177ede332bcc';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hamilton Brandon Gate', @venueCode = 'unknown_61fb1de2675f4cf69ead43d4562ded1b';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hastings County Court and Family Court', @venueCode = 'unknown_48ed70f734b7456db22fd393d20256cd';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Haverfordwest Magistrates Court', @venueCode = 'unknown_f8c75ab852724a5397124890f38583a5';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Hereford Justice Centre', @venueCode = 'unknown_98661d0df5264ddea842dc73e16a06d2';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'High Wycombe County Court and Family Court', @venueCode = 'unknown_9e967400081d479191cb87836362078b';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Horsham Magistrates Court', @venueCode = 'unknown_d21579d32b994374ac64173dfa7241e1';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Huntercombe Roehampton Hospital', @venueCode = 'unknown_202cc714c01d4a20a6e3d5b180ed786d';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Inverness Employment Tribunal', @venueCode = 'unknown_64df438d6a3644bba8e0d35786dc0028';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'King''s Lynn Magistrates Court and Family Court', @venueCode = 'unknown_10936e473a11450bb5a1e95d810ba978';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Laganside Courts', @venueCode = 'unknown_7c65ec6ec6ba43e6b18a5cf3969948f2';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Lancaster Court House', @venueCode = 'unknown_663c15516d834d7293d1e329d8d577af';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Land Registration', @venueCode = 'unknown_77361457e43548c0a40f28919884e400';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Leeds District Probate Registry', @venueCode = 'unknown_b87ac4dbb8bf47be80955c0ed670af1e';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Liverpool District Probate Registry', @venueCode = 'unknown_dfb9ef0f25e34fae8430e3df5c4c0d49';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Liverpool and Knowsley Magistrates Court', @venueCode = 'unknown_af98cb202d684987a5e6baf250b5e4f3';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'London (South) Employment Tribunal', @venueCode = 'unknown_ecec3eceecb642bb8a55e16ec1dbc93f';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'London Circuit Commercial Court', @venueCode = 'unknown_2f9ef29daf564156827e8db100194816';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Mary Seacole House - Birmingham', @venueCode = 'unknown_a8a5c11e2f6c47809d6ae1bdbf2b7c0c';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Mental Health Tribunal', @venueCode = 'unknown_9fa73c3d0a8347fa8c1a1aaf094cc7f6';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Middlesbrough County Court', @venueCode = 'unknown_7173c72bad374e70b9986aaf966e15a5';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Midlands Property Tribunal', @venueCode = 'unknown_1c7a61f11ad84362bbc8bbc0eafc3d9c';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newcastle upon Tyne Crown Court and Magistrates Court', @venueCode = 'unknown_29bcea37839246edb74e4482ee5843fd';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Newport', @venueCode = 'unknown_6db90c408aa0485b970b68485ba606d6';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Northampton Social Security and Child Support Tribunal', @venueCode = 'unknown_4a6a2abf476f4c0ab88ee0cf82907a70';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Northern Property Tribunal', @venueCode = 'unknown_5e1f3e0668e444e69cc0a96572ded743';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Nottingham Crown Court', @venueCode = 'unknown_84137f5818b24eaeb1808d5d87e605cf';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Nottingham Justice Centre', @venueCode = 'unknown_4ca2b18fffe84b008f08d23797a6ab36';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Nottingham Social Security and Child Support Tribunal', @venueCode = 'unknown_c3be6c2ae6c64672ad4ee7cb02cdcaec';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Residential Property Tribunal', @venueCode = 'unknown_614b12cc6f4e44d9846f3bdfc4db9ac4';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Skipton Magistrates Court', @venueCode = 'unknown_a8d3c3f314614e5396330cc5295d1eb9';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'South Tyneside Magistrates Court', @venueCode = 'unknown_0f81466af35045bb88d958d8408abf93';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Southend Magistrates Court', @venueCode = 'unknown_074398cd20244df99297010bf4c1f703';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Staines County Court and Family Court', @venueCode = 'unknown_411ee81b0d314e7da0d4304712f6c9a7';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Stockport County Court and Family Court', @venueCode = 'unknown_779c44b95ef34bb9be9726eb44cf855d';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'War and Pension Tribunal', @venueCode = 'unknown_f65705d222454cd98ef8497f6a8778b9';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Warrington Crown Court', @venueCode = 'unknown_00ea055d51864cc0a300c4309d3769a0';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Warrington Magistrates Court', @venueCode = 'unknown_b8845c5f36de4523a6867dcc586d0df6';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Warwick Combined Court', @venueCode = 'unknown_ddf7e190c265475bbbd2b6432e3d0d1c';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wigan County Court and Family Court', @venueCode = 'unknown_79f4961e2c904df090571451dce910b3';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Worthing Magistrates Court', @venueCode = 'unknown_948adaa8da274ae4bc4db6c673218269';
EXEC #HearingVenue_UpdateVenueCode @venueName = 'Wrexham Magistrates Court', @venueCode = 'unknown_3756c85c548843d1868a5f88a7e69470';

DROP PROC #HearingVenue_UpdateVenueCode;
GO;

SELECT * FROM HearingVenue
SELECT * FROM VhBookings.dbo.HearingVenue WHERE VenueCode IS NULL
-- Change the next line to commit
COMMIT;
SET XACT_ABORT OFF