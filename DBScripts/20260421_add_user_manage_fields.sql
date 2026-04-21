IF COL_LENGTH('dbo.dm_User', 'RecognizedEducation') IS NULL
BEGIN
    ALTER TABLE dbo.dm_User
    ADD RecognizedEducation NVARCHAR(64) NULL;
END
GO

IF COL_LENGTH('dbo.dm_User', 'ResignDate') IS NULL
BEGIN
    ALTER TABLE dbo.dm_User
    ADD ResignDate DATE NULL;
END
GO

IF COL_LENGTH('dbo.dm_User', 'JobEffectiveDate') IS NULL
BEGIN
    ALTER TABLE dbo.dm_User
    ADD JobEffectiveDate DATE NULL;
END
GO
