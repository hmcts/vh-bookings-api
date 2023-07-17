-- You may need to uncomment the next line if the connection is not specific to a default database
USE VhBookings
BEGIN TRANSACTION;

-- INSERT Family Jurisdiction if it does not exist and then grab the new ID
BEGIN IF NOT EXISTS(SELECT * FROM dbo.Jurisdiction WHERE Name LIKE 'Family')
    BEGIN
        INSERT INTO dbo.Jurisdiction (Code, Name, IsLive, CreatedDate, UpdatedDate) VALUES ('Family', 'Family', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
    END
END
GO;

-- Store the family jurisdiction id to reference later
declare @familyJurisdictionId VARCHAR(max);
SELECT @familyJurisdictionId = id FROM dbo.Jurisdiction WHERE Name LIKE 'Family';
UPDATE dbo.CaseType SET ServiceId = 'ABA5', JurisdictionId = @familyJurisdictionId, UpdatedDate = CURRENT_TIMESTAMP WHERE Name LIKE 'Private Law';
GO;


CREATE OR ALTER PROC #Upsert_PrivateLawHearingTypes @hearingTypeName nvarchar(max), @hearingTypeCode varchar(450), @welshName nvarchar(max), @privateLawCaseTypeId int
As
BEGIN
    IF EXISTS (SELECT * FROM dbo.HearingType WHERE Name = @hearingTypeName AND CaseTypeId = @privateLawCaseTypeId)
        BEGIN PRINT ('Updating: ' + @hearingTypeName)
        UPDATE dbo.HearingType SET Code = TRIM(@hearingTypeCode), WelshName = TRIM(@welshName), UpdatedDate = CURRENT_TIMESTAMP WHERE Name = @hearingTypeName
        END
    ELSE
        BEGIN PRINT ('Adding: ' + @hearingTypeName)
        INSERT INTO dbo.HearingType (Name, CaseTypeId, Live, CreatedDate, UpdatedDate, Code, WelshName)
        VALUES
            (TRIM(@hearingTypeName), @privateLawCaseTypeId, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, TRIM(@hearingTypeCode), @welshName)
        END
END
GO;

declare @privateLawCaseTypeId int;
SELECT @privateLawCaseTypeId = id FROM dbo.CaseType WHERE Name LIKE 'Private Law';

EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Application', @hearingTypeCode = 'ABA5-APP', @welshName = N'Cais', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Case Management Conference', @hearingTypeCode = 'ABA5-CMC', @welshName = N'Cynhadledd Rheoli Achos', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Directions', @hearingTypeCode = NULL, @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'First hearing', @hearingTypeCode = 'ABA5-FHR', @welshName = N'Gwrandawiad Cyntaf', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Full hearing', @hearingTypeCode = NULL, @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Pre hearing review', @hearingTypeCode = 'ABA5-PHR', @welshName = N'Adolygiad Cyn Gwrandawiad', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Review', @hearingTypeCode = 'ABA5-REV', @welshName = N'Adolygiad', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Safeguarding Gatekeeping Appointment', @hearingTypeCode = 'ABA5-SGA', @welshName = N'Apwyntiad Neilltuo Diogelwch', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Judgment', @hearingTypeCode = 'ABA5-JMT', @welshName = N'Dyfarniad', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Human Rights Act Application', @hearingTypeCode = 'ABA5-HRA', @welshName = N'Cais dan y Ddeddf Hawliau Dynol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Further Case Management Hearing', @hearingTypeCode = 'ABA5-FCM', @welshName = N'Gwrandawiad Rheoli Achos Pellach', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Full/Final hearing', @hearingTypeCode = 'ABA5-FFH', @welshName = N'Gwrandawiad Llawn/Terfynol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Finding of Fact', @hearingTypeCode = 'ABA5-FOF', @welshName = N'Canfod y Ffeithiau', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Dispute Resolution Appointment', @hearingTypeCode = 'ABA5-DRA', @welshName = N'Apwyntiad Datrys Anghydfod', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Directions (First/Further)', @hearingTypeCode = 'ABA5-DIR', @welshName = N'Cyfarwyddiadau (Cyntaf/Pellach)', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Costs', @hearingTypeCode = 'ABA5-COS', @welshName = N'Costau', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Conciliation', @hearingTypeCode = 'ABA5-CON', @welshName = N'Cymodi', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Committal', @hearingTypeCode = 'ABA5-COM', @welshName = N'Traddodi', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Case Management Hearing', @hearingTypeCode = 'ABA5-CMH', @welshName = N'Gwrandawiad Rheoli Achos', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Breach', @hearingTypeCode = 'ABA5-BRE', @welshName = N'Torri Amodau', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Appeal', @hearingTypeCode = 'ABA5-APL', @welshName = N'Apêl', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Allocation', @hearingTypeCode = 'ABA5-ALL', @welshName = N'Dyrannu', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = '2nd Gatekeeping Appointment', @hearingTypeCode = 'ABA5-2GA', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Ground Rules Hearing', @hearingTypeCode = 'ABA5-GRH', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Neutral Evaluation Hearing', @hearingTypeCode = 'ABA5-NEH', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Permission Hearing', @hearingTypeCode = 'ABA5-PER', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Settlement Conference', @hearingTypeCode = 'ABA5-SCF', @welshName = NULL, @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Celebration hearing', @hearingTypeCode = 'ABA5-CHR', @welshName = N'Gwrandawiad Dathlu', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Interim Care Order', @hearingTypeCode = 'ABA5-ICO', @welshName = N'Gorchymyn Gofal Interim', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Interim Supervision Order', @hearingTypeCode = 'ABA5-ISO', @welshName = N'Gorchymyn Goruchwylio Interim', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Issues Resolution Hearing', @hearingTypeCode = 'ABA5-IRH', @welshName = N'Gwrandawiad Datrys Materion', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Preliminary (REMO)', @hearingTypeCode = 'ABA5-PRE', @welshName = N'Rhagarweiniol (REMO)', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Financial remedy first appointment', @hearingTypeCode = 'ABA5-FRF', @welshName = N'Apwyntiad cyntaf rhwymedi ariannol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Financial remedy directions', @hearingTypeCode = 'ABA5-FRD', @welshName = N'Cyfarwyddiadau rhwymedi ariannol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Financial remedy financial dispute resolution', @hearingTypeCode = 'ABA5-FRR', @welshName = N'Rhwymedi ariannol datrys anghydfod ariannol', @privateLawCaseTypeId = @privateLawCaseTypeId
EXEC #Upsert_PrivateLawHearingTypes @hearingTypeName = 'Financial remedy interim order', @hearingTypeCode = 'ABA5-FRI', @welshName = N'Gorchymyn interim rhwymedi ariannol', @privateLawCaseTypeId = @privateLawCaseTypeId

SELECT * FROM dbo.HearingType
WHERE CaseTypeId = @privateLawCaseTypeId;

-- Change the next line to commit
ROLLBACK;