IF COL_LENGTH('dbo.dt309_Materials', 'ReplacementMaterialId') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials ADD ReplacementMaterialId int NULL;
END
GO

IF COL_LENGTH('dbo.dt309_Materials', 'ReplacementDate') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials ADD ReplacementDate datetime NULL;
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_dt309_Materials_ReplacementMaterial'
)
BEGIN
    ALTER TABLE dbo.dt309_Materials
    ADD CONSTRAINT FK_dt309_Materials_ReplacementMaterial
        FOREIGN KEY (ReplacementMaterialId) REFERENCES dbo.dt309_Materials(Id);
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_dt309_Materials_ReplacementMaterialId_Active'
      AND object_id = OBJECT_ID('dbo.dt309_Materials')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_dt309_Materials_ReplacementMaterialId_Active
        ON dbo.dt309_Materials(ReplacementMaterialId)
        WHERE ReplacementMaterialId IS NOT NULL AND DelTime IS NULL;
END
GO
