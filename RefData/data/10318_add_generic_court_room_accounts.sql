SET XACT_ABORT ON
GO

BEGIN TRANSACTION

DECLARE @Id UNIQUEIDENTIFIER = 'c36a8e30-3db6-418f-ad13-4eca70954e59'

INSERT INTO JudiciaryPerson
(
    Id,
    PersonalCode,
    KnownAs,
    Surname,
    Fullname,
    Email,
    CreatedDate,
    UpdatedDate,
    IsGeneric
)
VALUES
(
    @Id,
    'VH-GENERIC-ACCOUNT-1',
    'CourtRoom',
    'Account1',
    'CourtRoom1',
    'genericcourtroom@hearings.reform.hmcts.net',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    1
)

SELECT * FROM VhBookings.dbo.JudiciaryPerson WHERE Id = @Id

GO

COMMIT
SET XACT_ABORT OFF