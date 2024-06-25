
MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ase', 'American Sign Language (ASL)', 'Iaith Arwyddion America (ASL)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bfi', 'British Sign Language (BSL)', 'Iaith Arwyddion Prydain (BSL)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ils', 'International Sign (IS)', 'Arwyddion Rhyngwladol (IS)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-dfr', 'Deaf Relay', 'Gwasanaeth Cyfnewid i Bobl Fyddar', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-dma', 'Deafblind manual alphabet', 'Wyddor Pobl Ddall a Byddar', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-hos', 'Hands on signing', 'Arwyddo trwy gyffyrddiad', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-lps', 'Lipspeaker', 'Gwefuslefarydd', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-mkn', 'Makaton', 'Makaton', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-ntr', 'Notetaker', 'Ysgrifennwr Nodiadau', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-pst', 'Palantypist / Speech to text', 'Palanteipydd / Llais-i-destun', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-sse', 'Speech Supported English (SSE)', 'Cefnogaeth Lleferydd Saesneg (SSE)', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sign-vfs', 'Visual frame signing', 'Arwyddo Gweledol', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 1, UpdatedDate = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live, CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 1,  Source.Live, GETDATE(), GETDATE());



MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('abr', 'Brong', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ach', 'Acholi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('afr', 'Afrikaans', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('aii', 'Assyrian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('aka', 'Akan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('amh', 'Amharic', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ara', 'Arabic', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ara-ame', 'Arabic Middle Eastern', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ara-ana', 'Arabic North African', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ara-mag', 'Maghreb', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('arq', 'Algerian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('aze', 'Azerbajani (aka Nth Azari)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bal', 'Baluchi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bam', 'Bambara', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bas', 'Bassa', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bel', 'Belorussian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bem', 'Benba (Bemba)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ben', 'Bengali', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ben-bsy', 'Bengali Sylheti', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ber', 'Berber', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bfz', 'Pahari', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bih', 'Bihari', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bin', 'Benin/Edo', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bjs', 'Bajan (West Indian)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bnt-kic', 'Kichagga', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bod', 'Tibetan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('btn', 'Bhutanese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('bul', 'Bulgarian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('byn', 'Bilin', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ceb', 'Cebuano', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ces', 'Czech', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cgg', 'Rukiga', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('che', 'Chechen', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cmn', 'Mandarin', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cpe', 'Creole (English)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cpf', 'Creole (French)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cpp', 'Creole (Portuguese)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('crp', 'Creole (Spanish)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ctg', 'Chittagonain', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('cym', 'Welsh', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('dan', 'Danish', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('deu', 'German', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('din', 'Dinka', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('div', 'Maldivian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('don', 'Toura', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('dua', 'Douala', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('dyu', 'Dioula', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('efi', 'Efik', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ell', 'Greek', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('est', 'Estonian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ewe', 'Ewe', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ewo', 'Ewondo', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fas', 'Farsi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fat', 'Fanti', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fij', 'Fijian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fra', 'French', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fra-faf', 'French African', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('fra-far', 'French Arabic', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ful', 'Fula', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('gaa', 'Ga', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('gjk', 'Kachi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('glg', 'Galician', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('guj', 'Gujarati', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hac', 'Gorani', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hak', 'Hakka', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hau', 'Hausa', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hbs', 'Serbo-Croatian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('heb', 'Hebrew', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('her', 'Herero', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hin', 'Hindi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hnd', 'Hindko', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hno', 'Northern Hindko', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hun', 'Hungarian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('hye', 'Armenian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ibb', 'Ibibio', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ibo', 'Igbo (Also Known As Ibo)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ilo', 'Ilocano', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ind', 'Indonesian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ish', 'Esan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('iso', 'Isoko', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ita', 'Italian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('jam', 'Jamaican', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('jav', 'Javanese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('jpn', 'Japanese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kas', 'Kashmiri', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kat', 'Georgian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kck', 'Khalanga', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kfr', 'Kutchi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('khm', 'Khmer', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kik', 'Kikuyu', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kin', 'Kinyarwandan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kir', 'Kyrgyz', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('knn', 'Konkani', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kon', 'Kikongo', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kor', 'Korean', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kri', 'Krio (Sierra Leone)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('krn', 'Sarpo', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kru', 'Kru', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('krx', 'Karon', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kur-fey', 'Feyli', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kur-kbr', 'Kurdish Bardini', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kur-kkr', 'Kurdish kurmanji', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('kur-ksr', 'Kurdish Sorani', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('laj', 'Lango', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lav', 'Latvian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lin', 'Lingala', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lit', 'Lithuanian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lub', 'Luba (Tshiluba)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('lug', 'Lugandan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('luo', 'Luo', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('luo-lah', 'Luo Acholi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('luo-lky', 'Luo Kenyan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('luo-llg', 'Luo Lango', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mal', 'Malayalam', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mar', 'Marathi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('men', 'Mende', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('min', 'Mina', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mkd', 'Macedonian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mku', 'Malinke', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mkw', 'Monokutuba', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mlt', 'Maltese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mnk', 'Mandinka', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mon', 'Mongolian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('msa', 'Malay', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('mya', 'Burmese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('myx', 'Masaaba', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nde', 'Ndebele', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nep', 'Nepali', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nld', 'Dutch', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nld-nfl', 'Flemish', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nld-nwf', 'West Flemish', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nor', 'Norwegian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nya', 'Chichewa', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nyn', 'Nyankole', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nyo', 'Runyoro', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('nzi', 'Nzima', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('orm', 'Oromo', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pag', 'Pangasinan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pam', 'Pampangan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pan', 'Punjabi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pan-pji', 'Punjabi Indian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pan-pjp', 'Punjabi Pakistani', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pat', 'Patois', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('phr', 'Pahari-Potwari', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pol', 'Polish', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('por', 'Portuguese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('prs', 'Dari', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('pus', 'Pushtu (Also Known As Pashto)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('rmm', 'Roma', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('rom', 'Romany', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ron', 'Romanian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ron-fmo', 'Moldovan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('run', 'Kirundi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('rus', 'Russian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('scl', 'Shina', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sgw', 'Gurage', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sin', 'Sinhalese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('skr', 'Saraiki (Seraiki)', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('skt', 'Sakata', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('slk', 'Slovak', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('slv', 'Slovenian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sna', 'Shona', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('snd', 'Sindhi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('snk', 'Soninke', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('som', 'Somali', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('spa', 'Spanish', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('spv', 'Kosli, Sambalpuri', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sqi', 'Albanian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('sus', 'Susu', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swa', 'Swahili', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swa-sbv', 'Swahili Bravanese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swa-skb', 'Swahili Kibajuni', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swe', 'Swedish', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('swh', 'Kiswahili', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('syl', 'Sylheti', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tai', 'Taiwanese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tam', 'Tamil', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tel', 'Telugu', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tem', 'Temne', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('teo', 'Ateso', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tgl', 'Tagalog', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tha', 'Thai', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tig', 'Tigre', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tir', 'Tigrinya', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tsn', 'Setswana', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ttj', 'Rutoro', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tuk', 'Turkmen', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('tur', 'Turkish', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('twi', 'Twi', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('uig', 'Uighur', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('ukr', 'Ukrainian', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('urd', 'Urdu', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('urh', 'Urohobo', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('uzb', 'Uzbek', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('vie', 'Vietnamese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('vsa', 'Visayan', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('wol', 'Wolof', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('xho', 'Xhosa', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('xog', 'Lusoga', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('yor', 'Yoruba', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('yue', 'Cantonese', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('zag', 'Zaghawa', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('zho-hok', 'Hokkein', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('zul', 'Zulu', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());


MERGE INTO InterpreterLanguage AS Target
USING (VALUES ('zza', 'Zaza', '', 1))
    AS Source (Code, Value, WelshValue, Live)
ON Target.Code = Source.Code
WHEN MATCHED THEN
    UPDATE SET Value = Source.Value, WelshValue = Source.WelshValue, Live = Source.Live, Type = 2
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Code, Value, WelshValue, Type, Live,  CreatedDate, UpdatedDate) VALUES (Source.Code, Source.Value, Source.WelshValue, 2,  Source.Live, GETDATE(), GETDATE());
    