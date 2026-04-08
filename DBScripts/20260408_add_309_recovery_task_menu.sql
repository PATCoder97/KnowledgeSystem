/* ---------------------------------------------------------------------------
   [309] Add user recovery work function
   - Register function uc309_RecoveryTask under SparePart menu
   - Clone function roles from parent SparePart node
   --------------------------------------------------------------------------- */

SET NOCOUNT ON;

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
        WHERE ControlName = 'uc309_RecoveryTask'
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
            NCHAR(22238) + NCHAR(25910) + NCHAR(20316) + NCHAR(26989),
            'uc309_RecoveryTask',
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
