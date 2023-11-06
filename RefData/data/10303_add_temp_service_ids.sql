SET XACT_ABORT ON
GO

BEGIN TRANSACTION

DECLARE @nonExpiringCaseTypes TABLE (name NVARCHAR(MAX));

INSERT INTO @nonExpiringCaseTypes
VALUES
    ('Court of Appeal Criminal Division'),
    ('GRC - Query Jurisdiction'),
    ('Upper Tribunal Tax'),
    ('Upper Tribunal Immigration & Asylum Chamber'),
    ('Upper Tribunal Administrative Appeals Chamber'),
    ('Employment Appeal Tribunal'),
    ('Upper Tribunal Lands Chamber'),
    ('Crime Crown Court');


DECLARE @counter1 INT = 1;
DECLARE @tempTable1 TABLE (id INT IDENTITY(1,1), name NVARCHAR(MAX));
INSERT INTO @tempTable1(name)
SELECT name FROM @nonExpiringCaseTypes;
DECLARE @i INT = 1, @max1 INT = (SELECT MAX(id) FROM @tempTable1);
WHILE @i <= @max1
BEGIN
    DECLARE @serviceId1 NVARCHAR(MAX) = 'VIHTMP' + CAST(@counter1 AS NVARCHAR(MAX));

    UPDATE ct
    SET ct.ServiceId = @serviceId1, ct.UpdatedDate = CURRENT_TIMESTAMP
    FROM VhBookings.dbo.CaseType ct
    INNER JOIN @tempTable1 tt
    ON ct.name = tt.name
    WHERE tt.id = @i;
    PRINT @serviceId1

    SET @i = @i + 1;
    SET @counter1 = @counter1 + 1;
END

DECLARE @expiringCaseTypes TABLE (name NVARCHAR(MAX));

INSERT INTO @expiringCaseTypes
VALUES
    ('Children Act'),
    ('Family Law Act'),
    ('Tribunal'),
    ('Civil'),
    ('Housing Act'),
    ('Housing & Planning Act'),
    ('Leasehold Enfranchisement'),
    ('Leasehold Management'),
    ('Park Homes'),
    ('Rents'),
    ('Right to buy'),
    ('GRC - Professional Regulations'),
    ('Placement'),
    ('GRC - EJ'),
    ('Family'),
    ('Business Lease Renewal'),
    ('Tenant Fees');

DECLARE @counter2 INT = 1;
DECLARE @tempTable2 TABLE (id INT IDENTITY(1,1), name NVARCHAR(MAX));
INSERT INTO @tempTable2(name)
SELECT name FROM @expiringCaseTypes;
DECLARE @j INT = 1, @max2 INT = (SELECT MAX(id) FROM @tempTable2);
WHILE @j <= @max2
BEGIN
    DECLARE @serviceId2 NVARCHAR(MAX) = 'ZZY1' + CAST(@counter2 AS NVARCHAR(MAX));

    UPDATE ct
    SET ct.ServiceId = @serviceId2, ct.UpdatedDate = CURRENT_TIMESTAMP
    FROM VhBookings.dbo.CaseType ct
    INNER JOIN @tempTable2 tt
    ON ct.name = tt.name
    WHERE tt.id = @j;
    PRINT @serviceId2

    SET @j = @j + 1;
    SET @counter2 = @counter2 + 1;
END

COMMIT
SET XACT_ABORT OFF