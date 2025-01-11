-- Tabla de usuarios
CREATE TABLE USUARIO (
    USU_ID INT IDENTITY(1,1) PRIMARY KEY,
    USU_NOMBRE VARCHAR(50) NOT NULL,
    USU_PASSWORD VARCHAR(128) NOT NULL,
    USU_EMAIL VARCHAR(50) NOT NULL,
    USU_FECHA_CREACION DATETIME NOT NULL,
    USU_ACTIVO BIT NOT NULL
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripci�n: Inserta un nuevo registro en la tabla USUARIO.
-- Entradas: @USU_NOMBRE VARCHAR(50), @USU_PASSWORD VARCHAR(128), 
--           @USU_EMAIL VARCHAR(50), @USU_FECHA_CREACION DATETIME, @USU_ACTIVO BIT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Usuario_Insertar
    @USU_NOMBRE VARCHAR(50),
    @USU_PASSWORD VARCHAR(128),
    @USU_EMAIL VARCHAR(50),
    @USU_FECHA_CREACION DATETIME,
    @USU_ACTIVO BIT,
    @NuevoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO USUARIO (USU_NOMBRE, USU_PASSWORD, USU_EMAIL, USU_FECHA_CREACION, USU_ACTIVO)
    VALUES (@USU_NOMBRE, @USU_PASSWORD, @USU_EMAIL, @USU_FECHA_CREACION, @USU_ACTIVO);
    SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripci�n: Devuelve todos los registros de la tabla USUARIO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de USUARIO.
-- ====================================================================================
CREATE PROCEDURE sp_Usuario_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM USUARIO;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripci�n: Devuelve un registro espec�fico de USUARIO basado en su ID.
-- Entradas: @USU_ID INT - ID del usuario.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Usuario_ObtenerPorId
    @USU_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM USUARIO WHERE USU_ID = @USU_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripci�n: Actualiza un registro en USUARIO basado en su ID.
-- Entradas: @USU_ID INT, @USU_NOMBRE VARCHAR(50), @USU_PASSWORD VARCHAR(128), 
--           @USU_EMAIL VARCHAR(50), @USU_ACTIVO BIT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Usuario_Actualizar
    @USU_ID INT,
    @USU_NOMBRE VARCHAR(50),
    @USU_PASSWORD VARCHAR(128),
    @USU_EMAIL VARCHAR(50),
    @USU_ACTIVO BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE USUARIO
    SET USU_NOMBRE = @USU_NOMBRE,
        USU_PASSWORD = @USU_PASSWORD,
        USU_EMAIL = @USU_EMAIL,
        USU_ACTIVO = @USU_ACTIVO
    WHERE USU_ID = @USU_ID;
    SELECT @USU_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 30/12/2024
-- Descripci�n: Elimina un registro de USUARIO basado en su ID.
-- Entradas: @USU_ID INT - ID del usuario.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Usuario_Eliminar
    @USU_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM USUARIO WHERE USU_ID = @USU_ID;
END
GO

-- ====================================================================================
-- Scripts de prueba de USUARIO
-- ====================================================================================

-- Declaraci�n de variables para pruebas
DECLARE @UsuarioId INT;
DECLARE @UsuarioNombre VARCHAR(50) = 'UsuarioDePrueba';
DECLARE @UsuarioPassword VARCHAR(128) = 'Contrase�aEncriptada';
DECLARE @UsuarioEmail VARCHAR(50) = 'usuario@prueba.com';
DECLARE @UsuarioFechaCreacion DATETIME = GETDATE();
DECLARE @UsuarioActivo BIT = 1;
DECLARE @UsuarioNuevoNombre VARCHAR(50) = 'UsuarioActualizado';

-- *** Validaci�n inicial: Mostrar todos los registros existentes ***
PRINT '*** Validaci�n inicial: Mostrar todos los registros existentes ***';
EXEC sp_Usuario_ObtenerTodos;

-- *** Prueba: Insertar un nuevo usuario ***
PRINT '*** Prueba: Insertar un nuevo usuario ***';
EXEC sp_Usuario_Insertar
    @USU_NOMBRE = @UsuarioNombre,
    @USU_PASSWORD = @UsuarioPassword,
    @USU_EMAIL = @UsuarioEmail,
    @USU_FECHA_CREACION = @UsuarioFechaCreacion,
    @USU_ACTIVO = @UsuarioActivo,
    @NuevoId = @UsuarioId OUTPUT;

PRINT 'ID del nuevo usuario insertado: ' + CAST(@UsuarioId AS NVARCHAR);

-- *** Validaci�n: Mostrar el registro reci�n insertado por ID ***
PRINT '*** Validaci�n: Mostrar el registro reci�n insertado por ID ***';
EXEC sp_Usuario_ObtenerPorId @USU_ID = @UsuarioId;

-- *** Prueba: Actualizar el usuario ***
PRINT '*** Prueba: Actualizar el usuario ***';
EXEC sp_Usuario_Actualizar
    @USU_ID = @UsuarioId,
    @USU_NOMBRE = @UsuarioNuevoNombre,
    @USU_PASSWORD = @UsuarioPassword,
    @USU_EMAIL = @UsuarioEmail,
    @USU_ACTIVO = @UsuarioActivo;

-- *** Validaci�n: Mostrar el registro actualizado ***
PRINT '*** Validaci�n: Mostrar el registro actualizado ***';
EXEC sp_Usuario_ObtenerPorId @USU_ID = @UsuarioId;

-- *** Prueba: Eliminar el usuario ***
PRINT '*** Prueba: Eliminar el usuario ***';
EXEC sp_Usuario_Eliminar @USU_ID = @UsuarioId;

-- *** Validaci�n: Intentar obtener el usuario eliminado ***
PRINT '*** Validaci�n: Intentar obtener el usuario eliminado ***';
EXEC sp_Usuario_ObtenerPorId @USU_ID = @UsuarioId;

-- *** Validaci�n final: Mostrar todos los registros restantes ***
PRINT '*** Validaci�n final: Mostrar todos los registros restantes ***';
EXEC sp_Usuario_ObtenerTodos;
