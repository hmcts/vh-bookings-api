USE vhbookings;
GO;
SET XACT_ABORT ON
GO

CREATE PROCEDURE #AddOrUpdateJudiciaryPerson
    @PersonalCode NVARCHAR(50),
    @Fullname NVARCHAR(100),
    @Email NVARCHAR(100)
AS
BEGIN
    MERGE INTO JudiciaryPerson AS Target
    USING (VALUES (newid(), @PersonalCode, 'Court', 'Judge', @Fullname, @Email, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1))
        AS Source (Id, PersonalCode, KnownAs, Surname, Fullname, Email, CreatedDate, UpdatedDate, IsGeneric)
    ON Target.PersonalCode = Source.PersonalCode
    WHEN MATCHED THEN
        UPDATE SET KnownAs = Source.KnownAs, Surname = Source.Surname, Fullname = Source.Fullname, Email = Source.Email, UpdatedDate = Source.UpdatedDate
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Id, PersonalCode, KnownAs, Surname, Fullname, Email, CreatedDate, UpdatedDate, IsGeneric)
        VALUES (Source.Id, Source.PersonalCode, Source.KnownAs, Source.Surname, Source.Fullname, Source.Email, Source.CreatedDate, Source.UpdatedDate, Source.IsGeneric);
END
GO

BEGIN TRANSACTION

EXEC #AddOrUpdateJudiciaryPerson @PersonalCode = 'UpperTax.Judge1', @Fullname = 'UpperTax.Judge1', @Email = 'UpperTax.Judge1@hearings.reform.hmcts.net'
EXEC #AddOrUpdateJudiciaryPerson @PersonalCode = 'UpperTax.Judge2', @Fullname = 'UpperTax.Judge2', @Email = 'UpperTax.Judge2@hearings.reform.hmcts.net'
EXEC #AddOrUpdateJudiciaryPerson @PersonalCode = 'UpperTax.Judge3', @Fullname = 'UpperTax.Judge3', @Email = 'UpperTax.Judge3@hearings.reform.hmcts.net'
EXEC #AddOrUpdateJudiciaryPerson @PersonalCode = 'UpperTax.Judge4', @Fullname = 'UpperTax.Judge4', @Email = 'UpperTax.Judge4@hearings.reform.hmcts.net'
EXEC #AddOrUpdateJudiciaryPerson @PersonalCode = 'UpperTax.Judge5', @Fullname = 'UpperTax.Judge5', @Email = 'UpperTax.Judge5@hearings.reform.hmcts.net'

COMMIT

SET XACT_ABORT OFF