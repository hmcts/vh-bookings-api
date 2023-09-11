SET XACT_ABORT ON
BEGIN TRANSACTION;

-- INSERT Family Jurisdiction if it does not exist and then grab the new ID
BEGIN IF NOT EXISTS(SELECT * FROM dbo.Jurisdiction WHERE Name LIKE 'Family')
    BEGIN
        SET IDENTITY_INSERT dbo.Jurisdiction ON
        INSERT INTO dbo.Jurisdiction (Id, Code, Name, IsLive, CreatedDate, UpdatedDate) VALUES (3, 'Family', 'Family', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
        SET IDENTITY_INSERT dbo.Jurisdiction OFF
    END
END
GO;

-- Store the family jurisdiction id to reference later
declare @familyJurisdictionId VARCHAR(max);
SELECT @familyJurisdictionId = id FROM dbo.Jurisdiction WHERE Name LIKE 'Family';
UPDATE dbo.CaseType SET ServiceId = 'ABA5', JurisdictionId = @familyJurisdictionId, UpdatedDate = CURRENT_TIMESTAMP WHERE Name LIKE 'Private Law';
GO;


CREATE OR ALTER PROC #Upsert_PrivateLawHearingTypes @id int, @hearingTypeName nvarchar(max), @hearingTypeCode varchar(450), @welshName nvarchar(max), @privateLawCaseTypeId int
As
BEGIN
    IF EXISTS (SELECT * FROM dbo.HearingType WHERE Name = @hearingTypeName AND CaseTypeId = @privateLawCaseTypeId)
        BEGIN PRINT ('Updating: ' + @hearingTypeName)
        UPDATE dbo.HearingType SET Code = TRIM(@hearingTypeCode), WelshName = TRIM(@welshName), UpdatedDate = CURRENT_TIMESTAMP WHERE Name = @hearingTypeName AND CaseTypeId = @privateLawCaseTypeId
        END
    ELSE
        BEGIN PRINT ('Adding: ' + @hearingTypeName)
        SET IDENTITY_INSERT dbo.HearingType ON
        INSERT INTO dbo.HearingType (Id, Name, CaseTypeId, Live, CreatedDate, UpdatedDate, Code, WelshName)
        VALUES
            (@id, TRIM(@hearingTypeName), @privateLawCaseTypeId, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, TRIM(@hearingTypeCode), @welshName)
        SET IDENTITY_INSERT dbo.HearingType OFF
        END
END
GO;

declare @privateLawCaseTypeId int;
SELECT @privateLawCaseTypeId = id FROM dbo.CaseType WHERE Name LIKE 'Private Law';

EXEC #Upsert_PrivateLawHearingTypes @id = 147, @hearingTypeName = 'Application', @hearingTypeCode = 'ABA5-APP', @welshName = N'Cais', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 148, @hearingTypeName = 'Case Management Conference', @hearingTypeCode = 'ABA5-CMC', @welshName = N'Cynhadledd Rheoli Achos', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 149, @hearingTypeName = 'Directions', @hearingTypeCode = NULL, @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 150, @hearingTypeName = 'First hearing', @hearingTypeCode = 'ABA5-FHR', @welshName = N'Gwrandawiad Cyntaf', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 151, @hearingTypeName = 'Full hearing', @hearingTypeCode = NULL, @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 152, @hearingTypeName = 'Pre hearing review', @hearingTypeCode = 'ABA5-PHR', @welshName = N'Adolygiad Cyn Gwrandawiad', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 153, @hearingTypeName = 'Review', @hearingTypeCode = 'ABA5-REV', @welshName = N'Adolygiad', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 303, @hearingTypeName = 'Safeguarding Gatekeeping Appointment', @hearingTypeCode = 'ABA5-SGA', @welshName = N'Apwyntiad Neilltuo Diogelwch', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 304, @hearingTypeName = 'Judgment', @hearingTypeCode = 'ABA5-JMT', @welshName = N'Dyfarniad', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 305, @hearingTypeName = 'Human Rights Act Application', @hearingTypeCode = 'ABA5-HRA', @welshName = N'Cais dan y Ddeddf Hawliau Dynol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 306, @hearingTypeName = 'Further Case Management Hearing', @hearingTypeCode = 'ABA5-FCM', @welshName = N'Gwrandawiad Rheoli Achos Pellach', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 307, @hearingTypeName = 'Full/Final hearing', @hearingTypeCode = 'ABA5-FFH', @welshName = N'Gwrandawiad Llawn/Terfynol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 308, @hearingTypeName = 'Finding of Fact', @hearingTypeCode = 'ABA5-FOF', @welshName = N'Canfod y Ffeithiau', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 309, @hearingTypeName = 'Dispute Resolution Appointment', @hearingTypeCode = 'ABA5-DRA', @welshName = N'Apwyntiad Datrys Anghydfod', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 310, @hearingTypeName = 'Directions (First/Further)', @hearingTypeCode = 'ABA5-DIR', @welshName = N'Cyfarwyddiadau (Cyntaf/Pellach)', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 311, @hearingTypeName = 'Costs', @hearingTypeCode = 'ABA5-COS', @welshName = N'Costau', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 312, @hearingTypeName = 'Conciliation', @hearingTypeCode = 'ABA5-CON', @welshName = N'Cymodi', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 313, @hearingTypeName = 'Committal', @hearingTypeCode = 'ABA5-COM', @welshName = N'Traddodi', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 314, @hearingTypeName = 'Case Management Hearing', @hearingTypeCode = 'ABA5-CMH', @welshName = N'Gwrandawiad Rheoli Achos', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 315, @hearingTypeName = 'Breach', @hearingTypeCode = 'ABA5-BRE', @welshName = N'Torri Amodau', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 316, @hearingTypeName = 'Appeal', @hearingTypeCode = 'ABA5-APL', @welshName = N'Apêl', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 317, @hearingTypeName = 'Allocation', @hearingTypeCode = 'ABA5-ALL', @welshName = N'Dyrannu', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 318, @hearingTypeName = '2nd Gatekeeping Appointment', @hearingTypeCode = 'ABA5-2GA', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 319, @hearingTypeName = 'Ground Rules Hearing', @hearingTypeCode = 'ABA5-GRH', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 320, @hearingTypeName = 'Neutral Evaluation Hearing', @hearingTypeCode = 'ABA5-NEH', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 321, @hearingTypeName = 'Permission Hearing', @hearingTypeCode = 'ABA5-PER', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 322, @hearingTypeName = 'Settlement Conference', @hearingTypeCode = 'ABA5-SCF', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 323, @hearingTypeName = 'Celebration hearing', @hearingTypeCode = 'ABA5-CHR', @welshName = N'Gwrandawiad Dathlu', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 324, @hearingTypeName = 'Interim Care Order', @hearingTypeCode = 'ABA5-ICO', @welshName = N'Gorchymyn Gofal Interim', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 325, @hearingTypeName = 'Interim Supervision Order', @hearingTypeCode = 'ABA5-ISO', @welshName = N'Gorchymyn Goruchwylio Interim', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 326, @hearingTypeName = 'Issues Resolution Hearing', @hearingTypeCode = 'ABA5-IRH', @welshName = N'Gwrandawiad Datrys Materion', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 327, @hearingTypeName = 'Preliminary (REMO)', @hearingTypeCode = 'ABA5-PRE', @welshName = N'Rhagarweiniol (REMO)', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 328, @hearingTypeName = 'Financial remedy first appointment', @hearingTypeCode = 'ABA5-FRF', @welshName = N'Apwyntiad cyntaf rhwymedi ariannol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 329, @hearingTypeName = 'Financial remedy directions', @hearingTypeCode = 'ABA5-FRD', @welshName = N'Cyfarwyddiadau rhwymedi ariannol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 330, @hearingTypeName = 'Financial remedy financial dispute resolution', @hearingTypeCode = 'ABA5-FRR', @welshName = N'Rhwymedi ariannol datrys anghydfod ariannol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @id = 331, @hearingTypeName = 'Financial remedy interim order', @hearingTypeCode = 'ABA5-FRI', @welshName = N'Gorchymyn interim rhwymedi ariannol', @privateLawCaseTypeId = @privateLawCaseTypeId

SELECT * FROM dbo.HearingType
WHERE CaseTypeId = @privateLawCaseTypeId;

DROP PROC #Upsert_PrivateLawHearingTypes;
GO;

COMMIT
SET XACT_ABORT OFF