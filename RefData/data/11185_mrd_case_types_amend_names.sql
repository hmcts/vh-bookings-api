USE vhbookings
SET XACT_ABORT ON
GO;

BEGIN TRANSACTION;

UPDATE CaseType SET Name = 'Electronic Communications, Postal Services & Network Information Systems', UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'BAB5';
UPDATE CaseType SET Name = 'Pensions', UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'BAB9';
UPDATE CaseType SET Name = 'MP''s Expenses', UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'BDA1';

GO;

COMMIT;
SET XACT_ABORT OFF