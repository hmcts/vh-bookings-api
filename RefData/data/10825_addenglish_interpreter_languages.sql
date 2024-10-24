MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('eng','English', 'Saesneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());