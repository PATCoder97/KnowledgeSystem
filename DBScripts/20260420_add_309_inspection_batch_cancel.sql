SET NOCOUNT ON;

IF COL_LENGTH('dbo.dt309_InspectionBatch', 'IsCancelled') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_InspectionBatch
    ADD IsCancelled BIT NOT NULL
        CONSTRAINT DF_dt309_InspectionBatch_IsCancelled DEFAULT (0);
END;
GO

IF COL_LENGTH('dbo.dt309_InspectionBatch', 'CancelledBy') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_InspectionBatch
    ADD CancelledBy VARCHAR(10) NULL;
END;
GO

IF COL_LENGTH('dbo.dt309_InspectionBatch', 'CancelledDate') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_InspectionBatch
    ADD CancelledDate DATETIME NULL;
END;
GO

IF COL_LENGTH('dbo.dt309_InspectionBatch', 'CancelReason') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_InspectionBatch
    ADD CancelReason NVARCHAR(500) NULL;
END;
GO

UPDATE dbo.dt309_InspectionBatch
SET IsCancelled = 0
WHERE IsCancelled IS NULL;
GO
