-- Remove InquilinoBloqueado column if it exists
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE Name = N'InquilinoBloqueado' AND Object_ID = Object_ID(N'dbo.DetalleTrabajo')
)
BEGIN
    PRINT 'Dropping column InquilinoBloqueado from DetalleTrabajo';
    ALTER TABLE dbo.DetalleTrabajo DROP COLUMN InquilinoBloqueado;
END
ELSE
BEGIN
    PRINT 'Column InquilinoBloqueado does not exist, skipping';
END
