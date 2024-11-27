USE vhbookings
SET XACT_ABORT ON
GO;

BEGIN TRANSACTION;

UPDATE CaseType SET Name = 'Energy & Infrastructure', UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'BAB5';
UPDATE CaseType SET Name = 'Pensions Regulation', UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'BAB9';
UPDATE CaseType SET Name = 'FtT Tax Chamber inc MP''s Expenses', UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'BDA1';

GO;

COMMIT;
SET XACT_ABORT OFF