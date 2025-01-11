-- Tabla de permisos
CREATE TABLE PERMISO (
    PERM_ID INT IDENTITY(1,1) PRIMARY KEY,
    PERM_NOMBRE NVARCHAR(50) NOT NULL
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PERMISO.
-- Entradas: @PERM_NOMBRE NVARCHAR(50).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Permiso_Insertar
    @PERM_NOMBRE NVARCHAR(50),
    @NuevoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PERMISO (PERM_NOMBRE)
    VALUES (@PERM_NOMBRE);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Devuelve todos los registros de la tabla PERMISO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PERMISO.
-- ====================================================================================
CREATE PROCEDURE sp_Permiso_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PERMISO;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Devuelve un registro específico de PERMISO basado en su ID.
-- Entradas: @PERM_ID INT - ID del permiso.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Permiso_ObtenerPorId
    @PERM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PERMISO WHERE PERM_ID = @PERM_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Actualiza un registro en PERMISO basado en su ID.
-- Entradas: @PERM_ID INT, @PERM_NOMBRE NVARCHAR(50).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Permiso_Actualizar
    @PERM_ID INT,
    @PERM_NOMBRE NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PERMISO
    SET PERM_NOMBRE = @PERM_NOMBRE
    WHERE PERM_ID = @PERM_ID;
    SELECT @PERM_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Elimina un registro de PERMISO basado en su ID.
-- Entradas: @PERM_ID INT - ID del permiso.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Permiso_Eliminar
    @PERM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PERMISO WHERE PERM_ID = @PERM_ID;
END
GO

-- ====================================================================================
-- Scripts de prueba de PERMISO
-- ====================================================================================

-- Declaración de variables para pruebas
DECLARE @PermisoId INT;
DECLARE @PermisoNombre NVARCHAR(50) = 'Permiso de Prueba';
DECLARE @NuevoPermisoNombre NVARCHAR(50) = 'Permiso Actualizado de Prueba';

-- *** Validación inicial: Mostrar todos los registros existentes ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes ***';
EXEC sp_Permiso_ObtenerTodos;

-- *** Prueba: Insertar un nuevo permiso ***
PRINT '*** Prueba: Insertar un nuevo permiso ***';
EXEC sp_Permiso_Insertar
    @PERM_NOMBRE = @PermisoNombre,
    @NuevoId = @PermisoId OUTPUT;

PRINT 'ID del nuevo permiso insertado: ' + CAST(@PermisoId AS NVARCHAR);

-- *** Validación: Mostrar el registro recién insertado por ID ***
PRINT '*** Validación: Mostrar el registro recién insertado por ID ***';
EXEC sp_Permiso_ObtenerPorId @PERM_ID = @PermisoId;

-- *** Prueba: Actualizar el permiso ***
PRINT '*** Prueba: Actualizar el permiso ***';
EXEC sp_Permiso_Actualizar
    @PERM_ID = @PermisoId,
    @PERM_NOMBRE = @NuevoPermisoNombre;

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Permiso_ObtenerPorId @PERM_ID = @PermisoId;

-- *** Prueba: Eliminar el permiso ***
PRINT '*** Prueba: Eliminar el permiso ***';
EXEC sp_Permiso_Eliminar @PERM_ID = @PermisoId;

-- *** Validación: Intentar obtener el permiso eliminado ***
PRINT '*** Validación: Intentar obtener el permiso eliminado ***';
EXEC sp_Permiso_ObtenerPorId @PERM_ID = @PermisoId;

-- *** Validación final: Mostrar todos los registros restantes ***
PRINT '*** Validación final: Mostrar todos los registros restantes ***';
EXEC sp_Permiso_ObtenerTodos;
