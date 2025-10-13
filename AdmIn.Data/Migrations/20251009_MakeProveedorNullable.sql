-- Migration: Make ProveedorId nullable in TrabajoProveedor and update FK to SET NULL on delete
-- Created: 2025-10-09

SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @parentTable SYSNAME = 'TrabajoProveedor';
    DECLARE @referencedTable SYSNAME = 'Proveedor';
    DECLARE @schemaName SYSNAME = 'dbo';
    DECLARE @fkName SYSNAME;
    DECLARE @sql NVARCHAR(MAX);

    -- Find any FK on TrabajoProveedor that references Proveedor
    SELECT TOP 1 @fkName = fk.name
    FROM sys.foreign_keys fk
    JOIN sys.tables tParent ON fk.parent_object_id = tParent.object_id
    JOIN sys.tables tRef ON fk.referenced_object_id = tRef.object_id
    WHERE tParent.name = @parentTable AND tRef.name = @referencedTable;

    IF @fkName IS NOT NULL
    BEGIN
        SET @sql = N'ALTER TABLE ' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@parentTable) + N' DROP CONSTRAINT ' + QUOTENAME(@fkName) + N';';
        EXEC sp_executesql @sql;
    END

    -- Convert sentinel values (0) to NULL to avoid FK conflicts if any rows were set to 0
    IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id = t.object_id WHERE t.name = @parentTable AND c.name = 'ProveedorId')
    BEGIN
        -- Update rows where value is 0 (often used in UI code) to NULL
        SET @sql = N'UPDATE ' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@parentTable) + N' SET ProveedorId = NULL WHERE ProveedorId = 0;';
        EXEC sp_executesql @sql;
    END

    -- Alter column to allow NULL if currently NOT NULL
    IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.tables t ON c.object_id = t.object_id WHERE t.name = @parentTable AND c.name = 'ProveedorId' AND c.is_nullable = 0)
    BEGIN
        SET @sql = N'ALTER TABLE ' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@parentTable) + N' ALTER COLUMN ProveedorId INT NULL;';
        EXEC sp_executesql @sql;
    END

    -- Recreate FK with ON DELETE SET NULL so that if a proveedor row is deleted, the foreign key is set to NULL
    SET @sql = N'ALTER TABLE ' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@parentTable) + N'
                ADD CONSTRAINT FK_TrabajoProveedor_Proveedor FOREIGN KEY (ProveedorId) REFERENCES ' + QUOTENAME(@schemaName) + N'.' + QUOTENAME(@referencedTable) + N'(ProveedorID) ON DELETE SET NULL;';
    EXEC sp_executesql @sql;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    RAISERROR('Migration failed: %s', 16, 1, @ErrorMessage);
END CATCH;
GO

-- Post-migration notes:
-- 1) Update application models: TrabajoProveedor.ProveedorId should be nullable (int?).
-- 2) Ensure any code paths that set ProveedorId to 0 are updated to use NULL instead.
-- 3) After applying, the rejection flow can set ProveedorId = NULL without FK conflicts.
-- 4) If your Proveedor primary key column name differs from 'ProveedorID', update the script accordingly.
-- 5) Test thoroughly on a copy of production DB before applying.
