-- Tabla de roles
CREATE TABLE ROL (
    ROL_ID INT IDENTITY(1,1) PRIMARY KEY,
    ROL_NOMBRE NVARCHAR(50) NOT NULL
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Inserta un nuevo registro en la tabla ROL.
-- Entradas: @ROL_NOMBRE NVARCHAR(50).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Rol_Insertar
    @ROL_NOMBRE NVARCHAR(50),
    @NuevoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ROL (ROL_NOMBRE)
    VALUES (@ROL_NOMBRE);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Devuelve todos los registros de la tabla ROL.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de ROL.
-- ====================================================================================
CREATE PROCEDURE sp_Rol_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM ROL;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Devuelve un registro específico de ROL basado en su ID.
-- Entradas: @ROL_ID INT - ID del rol.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Rol_ObtenerPorId
    @ROL_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM ROL WHERE ROL_ID = @ROL_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Actualiza un registro en ROL basado en su ID.
-- Entradas: @ROL_ID INT, @ROL_NOMBRE NVARCHAR(50).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Rol_Actualizar
    @ROL_ID INT,
    @ROL_NOMBRE NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ROL
    SET ROL_NOMBRE = @ROL_NOMBRE
    WHERE ROL_ID = @ROL_ID;
    SELECT @ROL_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripción: Elimina un registro de ROL basado en su ID.
-- Entradas: @ROL_ID INT - ID del rol.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Rol_Eliminar
    @ROL_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM ROL WHERE ROL_ID = @ROL_ID;
END
GO

-- ====================================================================================
-- Scripts de prueba de ROL
-- ====================================================================================

-- Declaración de variables para pruebas
DECLARE @RolId INT;
DECLARE @RolNombre NVARCHAR(50) = 'Rol de Prueba';
DECLARE @NuevoRolNombre NVARCHAR(50) = 'Rol Actualizado de Prueba';

-- *** Validación inicial: Mostrar todos los registros existentes ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes ***';
EXEC sp_Rol_ObtenerTodos;

-- *** Prueba: Insertar un nuevo rol ***
PRINT '*** Prueba: Insertar un nuevo rol ***';
EXEC sp_Rol_Insertar
    @ROL_NOMBRE = @RolNombre,
    @NuevoId = @RolId OUTPUT;

PRINT 'ID del nuevo rol insertado: ' + CAST(@RolId AS NVARCHAR);

-- *** Validación: Mostrar el registro recién insertado por ID ***
PRINT '*** Validación: Mostrar el registro recién insertado por ID ***';
EXEC sp_Rol_ObtenerPorId @ROL_ID = @RolId;

-- *** Prueba: Actualizar el rol ***
PRINT '*** Prueba: Actualizar el rol ***';
EXEC sp_Rol_Actualizar
    @ROL_ID = @RolId,
    @ROL_NOMBRE = @NuevoRolNombre;

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Rol_ObtenerPorId @ROL_ID = @RolId;

-- *** Prueba: Eliminar el rol ***
PRINT '*** Prueba: Eliminar el rol ***';
EXEC sp_Rol_Eliminar @ROL_ID = @RolId;

-- *** Validación: Intentar obtener el rol eliminado ***
PRINT '*** Validación: Intentar obtener el rol eliminado ***';
EXEC sp_Rol_ObtenerPorId @ROL_ID = @RolId;

-- *** Validación final: Mostrar todos los registros restantes ***
PRINT '*** Validación final: Mostrar todos los registros restantes ***';
EXEC sp_Rol_ObtenerTodos;
