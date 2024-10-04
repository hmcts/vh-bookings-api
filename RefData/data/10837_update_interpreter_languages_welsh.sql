MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('abr','Brong', 'Brong', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ach','Acholi', 'Acholi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('afr','Afrikaans', 'Afrikaans', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('aii','Assyrian', 'Assyreg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('aka','Akan', 'Akan', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('amh','Amharic', 'Amhareg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ara','Arabic', 'Arabeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ara-ame','Arabic Middle Eastern', 'Arabeg – Dwyrain Canol', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ara-ana','Arabic North African', 'Arabeg – Gogledd Affrica', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ara-mag','Maghreb', 'Maghreb', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('arq','Algerian', 'Algereg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('aze','Azerbajani (aka Nth Azari)', 'Azerbajani (a elwir hefyd yn Nth Azari)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bal','Baluchi', 'Baluchi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bam','Bambara', 'Bambara', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bas','Bassa', 'Bassa', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bel','Belorussian', 'Belorwseg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bem','Benba (Bemba)', 'Benba (Bemba)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ben','Bengali', 'Bengali', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ben-bsy','Bengali Sylheti', 'Bengali Sylheti', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ber','Berber', 'Berber', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bfz','Pahari', 'Pahari', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bih','Bihari', 'Bihari', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bin','Benin/Edo', 'Benin/Edo', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bjs','Bajan (West Indian)', 'Bajan (India’r Gorllewin)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bnt-kic','Kichagga', 'Kichagga', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bod','Tibetan', 'Tibetaidd', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('btn','Bhutanese', 'Bhutaneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bul','Bulgarian', 'Bwlgareg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('byn','Bilin', 'Bilin', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ceb','Cebuano', 'Cebuano', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ces','Czech', 'Czech', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cgg','Rukiga', 'Rukiga', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('che','Chechen', 'Chechen', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cmn','Mandarin', 'Mandarin', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cpe','Creole (English)', 'Creole (Saesneg)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cpf','Creole (French)', 'Creole (Ffrangeg)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cpp','Creole (Portuguese)', 'Creole (Portwgeeg)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('crp','Creole (Spanish)', 'Creole (Sbaeneg)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ctg','Chittagonain', 'Chittagonain', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cym','Welsh', 'Cymraeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('dan','Danish', 'Daneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('deu','German', 'Almaeneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('din','Dinka', 'Dinka', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('div','Maldivian', 'Maldifeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('don','Toura', 'Toura', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('dua','Douala', 'Douala', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('dyu','Dioula', 'Dioula', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('efi','Efik', 'Efik', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ell','Greek', 'Groeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('est','Estonian', 'Estoneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ewe','Ewe', 'Ewe', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ewo','Ewondo', 'Ewondo', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fas','Farsi', 'Farsi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fat','Fanti', 'Fanti', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fij','Fijian', 'Fijian', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fra','French', 'Ffrangeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fra-faf','French African', 'Ffrangeg - Affricanaidd', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fra-far','French Arabic', 'Ffrangeg – Arabaidd', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ful','Fula', 'Fula', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('gaa','Ga', 'Ga', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('gjk','Kachi', 'Kachi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('glg','Galician', 'Galiseg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('guj','Gujarati', 'Gujarati', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hac','Gorani', 'Gorani', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hak','Hakka', 'Hakka', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hau','Hausa', 'Hausa', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hbs','Serbo-Croatian', 'Serbo-Croateg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('heb','Hebrew', 'Hebraeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('her','Herero', 'Herero', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hin','Hindi', 'Hindi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hnd','Hindko', 'Hindko', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hno','Northern Hindko', 'Hindko Gogleddol', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hun','Hungarian', 'Hwngareg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hye','Armenian', 'Armeneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ibb','Ibibio', 'Ibibio', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ibo','Igbo (Also Known As Ibo)', 'Igbo (a elwir hefyd yn Ibo)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ilo','Ilocano', 'Ilocano', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ind','Indonesian', 'Indoneseg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ish','Esan', 'Esan', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('iso','Isoko', 'Isoko', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ita','Italian', 'Eidaleg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('jam','Jamaican', 'Jamaicaidd', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('jav','Javanese', 'Javaneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('jpn','Japanese', 'Siapaneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kas','Kashmiri', 'Kashmiri', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kat','Georgian', 'Georgeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kck','Khalanga', 'Khalanga', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kfr','Kutchi', 'Kutchi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('khm','Khmer', 'Khmer', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kik','Kikuyu', 'Kikuyu', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kin','Kinyarwandan', 'Kinyarwandan', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kir','Kyrgyz', 'Kyrgyz', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('knn','Konkani', 'Konkani', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kon','Kikongo', 'Kikongo', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kor','Korean', 'Coreeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kri','Krio (Sierra Leone)', 'Krio (Sierra Leone)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('krn','Sarpo', 'Sarpo', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kru','Kru', 'Kru', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('krx','Karon', 'Karon', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kur-fey','Feyli', 'Feyli', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kur-kbr','Kurdish Bardini', 'Cwrdeg Bardini', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kur-kkr','Kurdish kurmanji', 'Cwrdeg Kurmanji', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kur-ksr','Kurdish Sorani', 'Cwrdeg Sorani', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('laj','Lango', 'Lango', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lav','Latvian', 'Latfieg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lin','Lingala', 'Lingala', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lit','Lithuanian', 'Lithiwaneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lub','Luba (Tshiluba)', 'Luba (Tshiluba)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lug','Lugandan', 'Lugandan', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('luo','Luo', 'Luo', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('luo-lah','Luo Acholi', 'Luo Acholi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('luo-lky','Luo Kenyan', 'Luo – Kenya', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('luo-llg','Luo Lango', 'Luo Lango', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mal','Malayalam', 'Malayalam', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mar','Marathi', 'Marathi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('men','Mende', 'Mende', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('min','Mina', 'Mina', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mkd','Macedonian', 'Macedoneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mku','Malinke', 'Malinke', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mkw','Monokutuba', 'Monokutuba', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mlt','Maltese', 'Maltaeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mnk','Mandinka', 'Mandinka', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mon','Mongolian', 'Mongoleg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('msa','Malay', 'Malay', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mya','Burmese', 'Bwrmeeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('myx','Masaaba', 'Masaaba', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nde','Ndebele', 'Ndebele', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nep','Nepali', 'Nepali', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nld','Dutch', 'Iseldireg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nld-nfl','Flemish', 'Fflemeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nld-nwf','West Flemish', 'Fflemeg Gorllewinol', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nor','Norwegian', 'Norwyeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nya','Chichewa', 'Chichewa', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nyn','Nyankole', 'Nyankole', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nyo','Runyoro', 'Runyoro', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nzi','Nzima', 'Nzima', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('orm','Oromo', 'Oromo', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pag','Pangasinan', 'Pangasinan', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pam','Pampangan', 'Pampangan', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pan','Punjabi', 'Punjabi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pan-pji','Punjabi Indian', 'Punjabi Indiaidd', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pan-pjp','Punjabi Pakistani', 'Punjabi Pakistani', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pat','Patois', 'Patois', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('phr','Pahari-Potwari', 'Pahari-Potwari', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pol','Polish', 'Pwyleg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('por','Portuguese', 'Portwgeeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('prs','Dari', 'Dari', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pus','Pushtu (Also Known As Pashto)', 'Pushtu (a elwir hefyd yn Pashto)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('rmm','Roma', 'Roma', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('rom','Romany', 'Romani', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ron','Romanian', 'Rwmaneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ron-fmo','Moldovan', 'Moldofeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('run','Kirundi', 'Kirundi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('rus','Russian', 'Rwseg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('scl','Shina', 'Shina', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sgw','Gurage', 'Gurage', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sin','Sinhalese', 'Sinhaleseg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('skr','Saraiki (Seraiki)', 'Saraiki (Seraiki)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('skt','Sakata', 'Sakata', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('slk','Slovak', 'Slovak', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('slv','Slovenian', 'Slofeneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sna','Shona', 'Shona', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('snd','Sindhi', 'Sindhi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('snk','Soninke', 'Soninke', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('som','Somali', 'Somali', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('spa','Spanish', 'Sbaeneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('spv','Kosli, Sambalpuri', 'Kosli, Sambalpuri', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sqi','Albanian', 'Albaniaidd', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sus','Susu', 'Susu', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swa','Swahili', 'Swahili', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swa-sbv','Swahili Bravanese', 'Swahili Bravanese', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swa-skb','Swahili Kibajuni', 'Swahili Kibajuni', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swe','Swedish', 'Swedeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swh','Kiswahili', 'Kiswahili', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('syl','Sylheti', 'Sylheti', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tai','Taiwanese', 'Taiwaneg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tam','Tamil', 'Tamil', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tel','Telugu', 'Telugu', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tem','Temne', 'Temne', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('teo','Ateso', 'Ateso', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tgl','Tagalog', 'Tagalog', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tha','Thai', 'Thai', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tig','Tigre', 'Tigre', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tir','Tigrinya', 'Tigrinya', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tsn','Setswana', 'Setswana', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ttj','Rutoro', 'Rutoro', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tuk','Turkmen', 'Turkmen', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tur','Turkish', 'Tyrceg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('twi','Twi', 'Twi', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('uig','Uighur', 'Uighur', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ukr','Ukrainian', 'Wcreineg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('urd','Urdu', 'Urdu', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('urh','Urohobo', 'Urohobo', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('uzb','Uzbek', 'Uzbek', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('vie','Vietnamese', 'Fietnameg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('vsa','Visayan', 'Visayan', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('wol','Wolof', 'Wolof', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('xho','Xhosa', 'Xhosa', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('xog','Lusoga', 'Lusoga', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('yor','Yoruba', 'Yoruba', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('yue','Cantonese', 'Cantoneeg', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('zag','Zaghawa', 'Zaghawa', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('zho-hok','Hokkein', 'Hokkein', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('zul','Zulu', 'Zulu', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('zza','Zaza', 'Zaza', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
