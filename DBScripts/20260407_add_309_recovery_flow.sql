SET NOCOUNT ON;

/* ---------------------------------------------------------------------------
   309 Recovery / Scrap flow
   - Add recovery flags to dt309_Materials
   - Create dt309_RecoveryTickets / dt309_RecoveryEvidence / dt309_RecoveryGuides
   - Create indexes
   - Register function uc309_RecoveryMgmt under SparePart menu
--------------------------------------------------------------------------- */

IF COL_LENGTH('dbo.dt309_Materials', 'BaseMaterialId') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials
    ADD BaseMaterialId INT NULL;
END;

IF COL_LENGTH('dbo.dt309_Materials', 'IsRecoveredOld') IS NULL
BEGIN
    ALTER TABLE dbo.dt309_Materials
    ADD IsRecoveredOld BIT NOT NULL
        CONSTRAINT DF_dt309_Materials_IsRecoveredOld DEFAULT (0);
END;

IF OBJECT_ID('dbo.dt309_RecoveryTickets', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt309_RecoveryTickets
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        TicketNo NVARCHAR(40) NOT NULL,
        IssueTransactionId INT NOT NULL,
        RestockInTransactionId INT NULL,
        NewMaterialId INT NOT NULL,
        OldBaseMaterialId INT NOT NULL,
        OldRecoveryMaterialId INT NULL,
        RecoveryOption VARCHAR(20) NOT NULL,
        Quantity FLOAT NOT NULL,
        SourceStorageId INT NOT NULL,
        RestockStorageId INT NULL,
        AssignedUserId VARCHAR(10) NULL,
        PlannedDisposeDate DATETIME NULL,
        ActualDisposeDate DATETIME NULL,
        Status VARCHAR(40) NOT NULL,
        Description NVARCHAR(1000) NULL,
        ResultNote NVARCHAR(1000) NULL,
        EvidenceSubmittedDate DATETIME NULL,
        CompletedBy VARCHAR(10) NULL,
        CompletedDate DATETIME NULL,
        CancelledBy VARCHAR(10) NULL,
        CancelledDate DATETIME NULL,
        CancelReason NVARCHAR(500) NULL,
        CreatedBy VARCHAR(10) NOT NULL,
        CreatedDate DATETIME NOT NULL,
        UpdatedBy VARCHAR(10) NULL,
        UpdatedDate DATETIME NULL
    );
END;

IF OBJECT_ID('dbo.dt309_RecoveryEvidence', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt309_RecoveryEvidence
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        RecoveryTicketId INT NOT NULL,
        ActualName NVARCHAR(256) NOT NULL,
        EncryptionName VARCHAR(64) NOT NULL,
        FileExt VARCHAR(16) NULL,
        UploadedBy VARCHAR(10) NULL,
        UploadedDate DATETIME NOT NULL,
        IsActive BIT NOT NULL
            CONSTRAINT DF_dt309_RecoveryEvidence_IsActive DEFAULT (1)
    );
END;

IF OBJECT_ID('dbo.dt309_RecoveryGuides', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt309_RecoveryGuides
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        Title NVARCHAR(256) NOT NULL,
        ActualName NVARCHAR(256) NOT NULL,
        EncryptionName VARCHAR(64) NOT NULL,
        FileExt VARCHAR(16) NULL,
        DisplayOrder INT NOT NULL
            CONSTRAINT DF_dt309_RecoveryGuides_DisplayOrder DEFAULT (1),
        IsActive BIT NOT NULL
            CONSTRAINT DF_dt309_RecoveryGuides_IsActive DEFAULT (1),
        UploadedBy VARCHAR(10) NULL,
        UploadedDate DATETIME NOT NULL
    );
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt309_RecoveryTickets_TicketNo'
      AND object_id = OBJECT_ID('dbo.dt309_RecoveryTickets')
)
BEGIN
    CREATE UNIQUE INDEX IX_dt309_RecoveryTickets_TicketNo
        ON dbo.dt309_RecoveryTickets (TicketNo);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt309_RecoveryTickets_NewMaterialId'
      AND object_id = OBJECT_ID('dbo.dt309_RecoveryTickets')
)
BEGIN
    CREATE INDEX IX_dt309_RecoveryTickets_NewMaterialId
        ON dbo.dt309_RecoveryTickets (NewMaterialId, Status, CreatedDate);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt309_RecoveryEvidence_Ticket'
      AND object_id = OBJECT_ID('dbo.dt309_RecoveryEvidence')
)
BEGIN
    CREATE INDEX IX_dt309_RecoveryEvidence_Ticket
        ON dbo.dt309_RecoveryEvidence (RecoveryTicketId, IsActive, UploadedDate);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt309_RecoveryGuides_ActiveOrder'
      AND object_id = OBJECT_ID('dbo.dt309_RecoveryGuides')
)
BEGIN
    CREATE INDEX IX_dt309_RecoveryGuides_ActiveOrder
        ON dbo.dt309_RecoveryGuides (IsActive, DisplayOrder, UploadedDate);
END;

DECLARE @ParentId INT;

SELECT TOP 1
    @ParentId = TRY_CAST(ValueT AS INT)
FROM dbo.sys_StaticValue
WHERE KeyT = 'RoleSparePart';

IF @ParentId IS NOT NULL
BEGIN
    DECLARE @FunctionId INT =
    (
        SELECT TOP 1 Id
        FROM dbo.dm_Function
        WHERE ControlName = 'uc309_RecoveryMgmt'
    );

    IF @FunctionId IS NULL
    BEGIN
        DECLARE @NewId INT = ISNULL((SELECT MAX(Id) FROM dbo.dm_Function), 0) + 1;
        DECLARE @Prioritize INT =
            ISNULL((SELECT MAX(ISNULL(Prioritize, 0)) FROM dbo.dm_Function WHERE IdParent = @ParentId), 0) + 1;

        INSERT INTO dbo.dm_Function
        (
            Id,
            IdParent,
            DisplayName,
            ControlName,
            Prioritize,
            Status,
            Images
        )
        VALUES
        (
            @NewId,
            @ParentId,
            N'回收管理',
            'uc309_RecoveryMgmt',
            @Prioritize,
            1,
            NULL
        );

        SET @FunctionId = @NewId;
    END;

    INSERT INTO dbo.dm_FunctionRole (IdFunction, IdRole)
    SELECT @FunctionId, SourceRole.IdRole
    FROM
    (
        SELECT DISTINCT fr.IdRole
        FROM dbo.dm_FunctionRole fr
        WHERE fr.IdFunction = @ParentId
           OR EXISTS
           (
               SELECT 1
               FROM dbo.dm_Function f
               WHERE f.Id = fr.IdFunction
                 AND f.IdParent = @ParentId
           )
    ) AS SourceRole
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.dm_FunctionRole existed
        WHERE existed.IdFunction = @FunctionId
          AND existed.IdRole = SourceRole.IdRole
    );
END;
