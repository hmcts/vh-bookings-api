SET XACT_ABORT ON
GO

CREATE PROCEDURE #Jurisdiction_CreateIfNotExist @id int, @name varchar(max)
AS
BEGIN
    IF NOT EXISTS(SELECT TOP 1 1 FROM Jurisdiction WHERE Name = @name)
        BEGIN 
            SET IDENTITY_INSERT Jurisdiction ON
            INSERT INTO Jurisdiction (id, Code, Name, IsLive, CreatedDate, UpdatedDate) 
                VALUES (@id, @name, @name, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
            SET IDENTITY_INSERT Jurisdiction OFF
        END
END
GO

CREATE PROCEDURE #CaseType_Update @name varchar(max), @serviceId nvarchar(max), @jurisdictionName nvarchar(max)
AS
BEGIN
    DECLARE @jurisdictionId int = (SELECT Id FROM Jurisdiction WHERE Name = @jurisdictionName)

    IF EXISTS(SELECT TOP 1 1 FROM CaseType WHERE Name = @name)
        BEGIN
            UPDATE CaseType SET ServiceId = @serviceId, JurisdictionId = @jurisdictionId, UpdatedDate = CURRENT_TIMESTAMP WHERE Name = @name
        END
    ELSE
        BEGIN
            Print ('WARNING! Could not find case type with the name: ' + @name);
        END
END
GO

BEGIN TRANSACTION

-- Add jurisdictions
EXEC #Jurisdiction_CreateIfNotExist @id = 4, @name = 'Health, Education and Social Care Chamber'
EXEC #Jurisdiction_CreateIfNotExist @id = 5, @name = 'General Regulatory Chamber'
EXEC #Jurisdiction_CreateIfNotExist @id = 6, @name = 'Immigration and Asylum Chamber'
EXEC #Jurisdiction_CreateIfNotExist @id = 7, @name = 'Property Chamber'
EXEC #Jurisdiction_CreateIfNotExist @id = 8, @name = 'War Pensions and Armed Forces Compensation Chamber'

-- Update case types
EXEC #CaseType_Update 'Adoption', 'ABA4', 'Family'
EXEC #CaseType_Update 'Asylum Support', 'BBA1', 'Social Entitlement Chamber'
EXEC #CaseType_Update 'Care Standards', 'BCA1', 'Health, Education and Social Care Chamber'
EXEC #CaseType_Update 'Criminal Injuries Compensation', 'BBA2', 'Social Entitlement Chamber'
EXEC #CaseType_Update 'Divorce', 'ABA1', 'Family'
EXEC #CaseType_Update 'Financial Remedy', 'ABA2', 'Family'
EXEC #CaseType_Update 'GRC - Charity', 'BAA2', 'General Regulatory Chamber'
EXEC #CaseType_Update 'GRC - Environment', 'BAA5', 'General Regulatory Chamber'
EXEC #CaseType_Update 'GRC - Estate Agents', 'BAA6', 'General Regulatory Chamber'
EXEC #CaseType_Update 'GRC - Gambling', 'BAA8', 'General Regulatory Chamber'
EXEC #CaseType_Update 'GRC - Immigration Services', 'BAA9', 'General Regulatory Chamber'
EXEC #CaseType_Update 'GRC - Information Rights', 'BAB1', 'General Regulatory Chamber'
EXEC #CaseType_Update 'Immigration and Asylum', 'BFA1', 'Immigration and Asylum Chamber'
EXEC #CaseType_Update 'Land Registration', 'BGA2', 'Property Chamber'
EXEC #CaseType_Update 'Mental Health', 'BCA2', 'Health, Education and Social Care Chamber'
EXEC #CaseType_Update 'Special Educational Needs and Disability', 'BCA4', 'Health, Education and Social Care Chamber'
EXEC #CaseType_Update 'War Pensions Appeals', 'BEA1', 'War Pensions and Armed Forces Compensation Chamber'

COMMIT

SET XACT_ABORT OFF