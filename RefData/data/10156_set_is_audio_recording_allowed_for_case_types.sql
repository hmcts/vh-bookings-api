SET XACT_ABORT ON
BEGIN TRANSACTION

UPDATE CaseType SET IsAudioRecordingAllowed = 0, UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'VIHTMP1'
UPDATE CaseType SET IsAudioRecordingAllowed = 0, UpdatedDate = CURRENT_TIMESTAMP WHERE ServiceId = 'VIHTMP8'

COMMIT
SET XACT_ABORT OFF