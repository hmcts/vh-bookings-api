USE vhbookings
SET XACT_ABORT ON

BEGIN TRANSACTION
BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.Jurisdiction WHERE Name = 'Upper Tribunal Tax and Chancery Chamber')
    INSERT INTO Jurisdiction (Code, Name, CreatedDate, UpdatedDate, IsLive)
    VALUES ('Upper Tribunal Tax and Chancery Chamber', 'Upper Tribunal Tax and Chancery Chamber', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1)
END
GO;



CREATE PROCEDURE #CREATE_CASE_TYPES_V11105 @CaseTypeId INT, @name varchar(max), @serviceId varchar(max)
AS
DECLARE @JurisdictionId INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Upper Tribunal Tax and Chancery Chamber')
BEGIN IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.CaseType WHERE Id = @CaseTypeId)
    BEGIN
        INSERT INTO CaseType (Id, Name, CreatedDate, UpdatedDate, JurisdictionId, Live, ServiceId)
        VALUES (@CaseTypeId, @name, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @JurisdictionId, 1, @serviceId)
    END
END
GO;


SET IDENTITY_INSERT CaseType ON
EXEC #CREATE_CASE_TYPES_V11105 75, 'Upper Tribunals Charity', 'BTA2';
EXEC #CREATE_CASE_TYPES_V11105 76, 'Upper Tribunals Notice of Reference Finance Services', 'BTA3';
EXEC #CREATE_CASE_TYPES_V11105 77, 'Upper Tribunals Notice of Reference Energy Market Decisions', 'BTA4';
EXEC #CREATE_CASE_TYPES_V11105 78, 'Upper Tribunals Notice of Reference Trade Remedies', 'BTA5';
EXEC #CREATE_CASE_TYPES_V11105 79, 'Upper Tribunals Notice of Appeal Trade Remedies ', 'BTA6';
EXEC #CREATE_CASE_TYPES_V11105 80, 'Upper Tribunals Notice of Appeal Financial Sanctions', 'BTA7';
EXEC #CREATE_CASE_TYPES_V11105 81, 'Upper Tribunals Judicial Review Applications (Tax)', 'BTA8';
EXEC #CREATE_CASE_TYPES_V11105 82, 'Upper Tribunals Agricultural Land and Drainage', 'BLA1';
EXEC #CREATE_CASE_TYPES_V11105 83, 'Upper Tribunals Land Registration', 'BLA2';
EXEC #CREATE_CASE_TYPES_V11105 84, 'Upper Tribunals First-Tier (Property Chamber)', 'BLA3';
EXEC #CREATE_CASE_TYPES_V11105 85, 'Upper Tribunals Leasehold Valuation Tribunal in Wales', 'BLA4';
EXEC #CREATE_CASE_TYPES_V11105 86, 'Upper Tribunals Residential Property in Wales', 'BLA5';
EXEC #CREATE_CASE_TYPES_V11105 87, 'Upper Tribunals Law Property Act 1925 Applications', 'BLA6';
EXEC #CREATE_CASE_TYPES_V11105 88, 'Upper Tribunals Ratings Appeals', 'BLA7';
EXEC #CREATE_CASE_TYPES_V11105 89, 'Upper Tribunals Notice of Reference', 'BLA8';
EXEC #CREATE_CASE_TYPES_V11105 90, 'Upper Tribunals Rights of Light Applications', 'BLA9';
EXEC #CREATE_CASE_TYPES_V11105 91, 'Upper Tribunals Absent Owner Applications', 'BLB1';
EXEC #CREATE_CASE_TYPES_V11105 92, 'Upper Tribunals Applications under s.33 Landlords and Tenant act 1987', 'BLB2';
EXEC #CREATE_CASE_TYPES_V11105 93, 'Upper Tribunals Environment', 'BKA4';
EXEC #CREATE_CASE_TYPES_V11105 94, 'Upper Tribunals Estate Agents', 'BKA5';
EXEC #CREATE_CASE_TYPES_V11105 95, 'Upper Tribunals Examination Boards', 'BKA6';
EXEC #CREATE_CASE_TYPES_V11105 96, 'Upper Tribunals Gambling Appeals', 'BKA7';
EXEC #CREATE_CASE_TYPES_V11105 97, 'Upper Tribunals Immigration Services', 'BKA8';
EXEC #CREATE_CASE_TYPES_V11105 98, 'Upper Tribunals Information Rights', 'BKA9';
EXEC #CREATE_CASE_TYPES_V11105 99, 'Upper Tribunals Driving Instructor', 'BKB1';
EXEC #CREATE_CASE_TYPES_V11105 100, 'Upper Tribunals Criminal Injuries Compensation', 'BKB2';
EXEC #CREATE_CASE_TYPES_V11105 101, 'Upper Tribunals Social Security and Child Support', 'BKB3';
EXEC #CREATE_CASE_TYPES_V11105 102, 'Upper Tribunals Care Standards', 'BKB4';
EXEC #CREATE_CASE_TYPES_V11105 103, 'Upper Tribunals Mental Health', 'BKB5';
EXEC #CREATE_CASE_TYPES_V11105 104, 'Upper Tribunals Primary Health Lists', 'BKB6';
EXEC #CREATE_CASE_TYPES_V11105 105, 'Upper Tribunals Special Educational Needs and Disability', 'BKB7';
EXEC #CREATE_CASE_TYPES_V11105 106, 'Upper Tribunals War Pensions Appeals', 'BKB8';
EXEC #CREATE_CASE_TYPES_V11105 107, 'Upper Tribunals Safeguarding decisions of Disclosure Barring Service', 'BKB9';
EXEC #CREATE_CASE_TYPES_V11105 108, 'Upper Tribunals Transport from the decisions of the traffic commissioners', 'BKC1';
EXEC #CREATE_CASE_TYPES_V11105 109, 'Upper Tribunals Food Safety', 'BKC2';
EXEC #CREATE_CASE_TYPES_V11105 110, 'Upper Tribunals Professional Regulation', 'BKC3';
EXEC #CREATE_CASE_TYPES_V11105 111, 'Upper Tribunals Welfare of Animals', 'BKC4';
EXEC #CREATE_CASE_TYPES_V11105 112, 'Upper Tribunals Pensions Regulation', 'BKC5';
EXEC #CREATE_CASE_TYPES_V11105 113, 'Upper Tribunals Judicial Review Applications (AAC)', 'BKC6';
EXEC #CREATE_CASE_TYPES_V11105 114, 'Employment Appeals Employment Claims', 'BMA1';
EXEC #CREATE_CASE_TYPES_V11105 115, 'Employment Appeals Central Arbitration Committee', 'BMA2';
EXEC #CREATE_CASE_TYPES_V11105 116, 'Employment Appeals Certification Officer', 'BMA3';
DECLARE @JurisdictionId INT = (SELECT Id FROM dbo.Jurisdiction WHERE Code = 'Upper Tribunal Tax and Chancery Chamber')
UPDATE CaseType SET Name = 'Upper Tribunals Tax Appeals & MP’s Expenses', ServiceId = 'BTA1', JurisdictionId = @JurisdictionId WHERE Name = 'Upper Tribunal Tax';
UPDATE CaseType SET Name = 'Upper Tribunals Immigration and Asylum Appeals', ServiceId = 'BIA1', JurisdictionId = @JurisdictionId WHERE Name = 'Upper Tribunal Immigration & Asylum Chamber';

GO;
SET IDENTITY_INSERT CaseType OFF

SELECT * FROM CaseType WHERE ID >= 75

DROP PROC #CREATE_CASE_TYPES_V11105;
GO;

COMMIT
SET XACT_ABORT OFF