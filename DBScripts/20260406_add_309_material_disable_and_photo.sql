IF COL_LENGTH('dbo.dt309_Materials', 'IsDisable') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials ADD IsDisable bit NULL;
END
GO

IF COL_LENGTH('dbo.dt309_Materials', 'DisabledBy') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials ADD DisabledBy varchar(10) NULL;
END
GO

IF COL_LENGTH('dbo.dt309_Materials', 'DisabledDate') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials ADD DisabledDate datetime NULL;
END
GO

IF COL_LENGTH('dbo.dt309_Materials', 'EnabledBy') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials ADD EnabledBy varchar(10) NULL;
END
GO

IF COL_LENGTH('dbo.dt309_Materials', 'EnabledDate') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials ADD EnabledDate datetime NULL;
END
GO

IF OBJECT_ID('dbo.dt309_MaterialPhoto', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt309_MaterialPhoto
    (
        Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MaterialId int NOT NULL,
        EncryptionName varchar(64) NOT NULL,
        ActualName nvarchar(256) NOT NULL,
        UploadedBy varchar(10) NULL,
        UploadedDate datetime NOT NULL,
        IsActive bit NOT NULL CONSTRAINT DF_dt309_MaterialPhoto_IsActive DEFAULT(1)
    );

    CREATE INDEX IX_dt309_MaterialPhoto_MaterialId ON dbo.dt309_MaterialPhoto(MaterialId);
END
GO

UPDATE dbo.dt309_Materials
SET IsDisable = ISNULL(IsDisable, 0)
WHERE IsDisable IS NULL;
GO
