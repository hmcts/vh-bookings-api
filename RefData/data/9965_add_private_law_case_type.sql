-- You may need to uncomment the next line if the connection is not specific to a default database
-- USE VhBookings
BEGIN TRANSACTION;

-- INSERT Family Jurisdiction if it does not exist and then grab the new ID
BEGIN IF NOT EXISTS(
    SELECT
        *
    FROM
        dbo.Jurisdiction
    WHERE
        Name LIKE 'Family'
) BEGIN
INSERT INTO
    dbo.Jurisdiction (Code, Name, IsLive, CreatedDate, UpdatedDate)
VALUES
    (
        'Family',
        'Family',
        1,
        CURRENT_TIMESTAMP,
        CURRENT_TIMESTAMP
    );

END
END -- Store the family jurisdiction id to reference later
declare @familyJurisdictionId VARCHAR(max);

SELECT
    @familyJurisdictionId = id
FROM
    dbo.Jurisdiction
WHERE
    Name LIKE 'Family';

SELECT
    @familyJurisdictionId
UPDATE
    dbo.CaseType
SET
    ServiceId = 'ABA5',
    JurisdictionId = @familyJurisdictionId,
    UpdatedDate = CURRENT_TIMESTAMP
WHERE
    Name LIKE 'Private Law';

declare @privateLawCaseTypeId VARCHAR(max);

SELECT
    @privateLawCaseTypeId = id
FROM
    dbo.CaseType
WHERE
    Name LIKE 'Private Law';

-- CANNOT FIND THIS! IS THIS CONFUSED WITH ANOTHER ROW?
UPDATE
    dbo.HearingType
SET
    Code = 'ABA5-APP',
    WelshName = 'Cais',
    UpdatedDate = CURRENT_TIMESTAMP
WHERE
    CaseTypeId = @privateLawCaseTypeId
    AND Name LIKE 'Appeals'
UPDATE
    dbo.HearingType
SET
    Code = 'ABA5-CMC',
    WelshName = 'Cynhadledd Rheoli Achos',
    UpdatedDate = CURRENT_TIMESTAMP
WHERE
    CaseTypeId = @privateLawCaseTypeId
    AND Name LIKE 'Case Management Conference'
UPDATE
    dbo.HearingType
SET
    Code = 'ABA5-FHR',
    WelshName = 'Gwrandawiad Cyntaf',
    UpdatedDate = CURRENT_TIMESTAMP
WHERE
    CaseTypeId = @privateLawCaseTypeId
    AND Name LIKE 'First Hearing'
UPDATE
    dbo.HearingType
SET
    Code = 'ABA5-PHR',
    WelshName = 'Adolygiad Cyn Gwrandawiad',
    UpdatedDate = CURRENT_TIMESTAMP
WHERE
    CaseTypeId = @privateLawCaseTypeId
    AND Name LIKE 'Pre hearing review'
UPDATE
    dbo.HearingType
SET
    Code = 'ABA5-REV',
    WelshName = 'Adolygiad Cyn Gwrandawiad',
    UpdatedDate = CURRENT_TIMESTAMP
WHERE
    CaseTypeId = @privateLawCaseTypeId
    AND Name LIKE 'Review' IF OBJECT_ID('tempdb..#tmp_NewPrivateLawCaseTypes') IS NOT NULL BEGIN DROP TABLE #tmp_NewPrivateLawCaseTypes
END -- Create a temp table of all the private law case types to loop through
CREATE TABLE #tmp_NewPrivateLawCaseTypes
(
    Name varchar(max),
    Code varchar(max),
    WelshName varchar(max),
)
INSERT INTO
    #tmp_NewPrivateLawCaseTypes
    (Name, Code, WelshName)
VALUES
    (
        'Safeguarding Gatekeeping Appointment',
        'ABA5-SGA',
        'Apwyntiad Neilltuo Diogelwch'
    ),
    ('Judgment', 'ABA5-JMT', 'Dyfarniad'),
    (
        'Human Rights Act Application',
        'ABA5-HRA',
        'Cais dan y Ddeddf Hawliau Dynol'
    ),
    (
        'Further Case Management Hearing',
        'ABA5-FCM',
        'Gwrandawiad Rheoli Achos Pellach'
    ),
    (
        'Full/Final hearing',
        'ABA5-FFH',
        'Gwrandawiad Llawn/Terfynol'
    ),
    (
        'Finding of Fact',
        'ABA5-FOF',
        'Canfod y Ffeithiau'
    ),
    (
        'Dispute Resolution Appointment',
        'ABA5-DRA',
        'Apwyntiad Datrys Anghydfod'
    ),
    (
        'Directions (First/Further)',
        'ABA5-DIR',
        'Cyfarwyddiadau (Cyntaf/Pellach)'
    ),
    ('Costs	ABA5-COS', 'Costau', NULL),
    ('Conciliation', 'ABA5-CON', 'Cymodi'),
    ('Committal', 'ABA5-COM', 'Traddodi'),
    (
        'Case Management Hearing',
        'ABA5-CMH',
        'Gwrandawiad Rheoli Achos'
    ),
    ('Breach', 'ABA5-BRE', 'Torri Amodau'),
    ('Appeal', 'ABA5-APL', 'Apêl'),
    ('Allocation', 'ABA5-ALL', 'Dyrannu'),
    ('2nd Gatekeeping Appointment', 'ABA5-2GA', NULL),
    ('Ground Rules Hearing', 'ABA5-GRH', NULL),
    ('Neutral Evaluation Hearing', 'ABA5-NEH', NULL),
    ('Permission Hearing', 'ABA5-PER', NULL),
    ('Settlement Conference', 'ABA5-SCF', NULL),
    (
        'Celebration hearing',
        'ABA5-CHR',
        'Gwrandawiad Dathlu'
    ),
    (
        'Interim Care Order',
        'ABA5-ICO',
        'Gorchymyn Gofal Interim'
    ),
    (
        'Interim Supervision Order',
        'ABA5-ISO',
        'Gorchymyn Goruchwylio Interim'
    ),
    (
        'Issues Resolution Hearing',
        'ABA5-IRH',
        'Gwrandawiad Datrys Materion'
    ),
    (
        'Preliminary (REMO)',
        'ABA5-PRE',
        'Rhagarweiniol (REMO)'
    ),
    (
        'Financial remedy first appointment',
        'ABA5-FRF',
        'Apwyntiad cyntaf rhwymedi ariannol'
    ),
    (
        'Financial remedy directions',
        'ABA5-FRD',
        'Cyfarwyddiadau rhwymedi ariannol'
    ),
    (
        'Financial remedy financial dispute resolution',
        'ABA5-FRR',
        'Rhwymedi ariannol datrys anghydfod ariannol'
    ),
    (
        'Financial remedy interim order',
        'ABA5-FRI',
        'Gorchymyn interim rhwymedi ariannol'
    ) DECLARE @tmpName VARCHAR(max),
    @tmpCode VARCHAR(max),
    @tmpWelshName VARCHAR(max);

DECLARE @cur CURSOR
SET
    @cur = CURSOR FOR
SELECT
    TRIM(Name),
    TRIM(Code),
    TRIM(WelshName)
FROM
    #tmp_NewPrivateLawCaseTypes;
    OPEN @cur;

FETCH NEXT
FROM
    @cur INTO @tmpName,
    @tmpCode,
    @tmpWelshName;

WHILE @ @FETCH_STATUS = 0 BEGIN IF EXISTS (
    SELECT
        *
    FROM
        dbo.HearingType
    WHERE
        Name = @tmpName
        AND CaseTypeId = @privateLawCaseTypeId
) BEGIN PRINT ('Updating: ' + @tmpName)
UPDATE
    dbo.HearingType
SET
    Code = TRIM(@tmpCode),
    WelshName = TRIM(@tmpWelshName)
WHERE
    Name = @tmpName
END
ELSE BEGIN PRINT ('Adding: ' + @tmpName)
INSERT INTO
    dbo.HearingType (
        Name,
        CaseTypeId,
        Live,
        CreatedDate,
        UpdatedDate,
        Code,
        WelshName
    )
VALUES
    (
        TRIM(@tmpName),
        @privateLawCaseTypeId,
        1,
        CURRENT_TIMESTAMP,
        CURRENT_TIMESTAMP,
        @tmpCode,
        @tmpWelshName
    )
END FETCH NEXT
FROM
    @cur INTO @tmpName,
    @tmpCode,
    @tmpWelshName;

END CLOSE @cur;

DEALLOCATE @cur;

DROP TABLE #tmp_NewPrivateLawCaseTypes
SELECT
    *
FROM
    dbo.HearingType
WHERE
    CaseTypeId = @privateLawCaseTypeId;

-- Change the next line to commit
ROLLBACK;