-- ====================================================================================
-- Tabla intermedia entre usuarios y roles
-- ====================================================================================
CREATE TABLE USUARIO_ROL (
    USU_ID INT NOT NULL,
    ROL_ID INT NOT NULL,
    PRIMARY KEY (USU_ID, ROL_ID), -- Clave primaria compuesta
    FOREIGN KEY (USU_ID) REFERENCES USUARIO(USU_ID), -- Clave for�nea hacia USUARIO
    FOREIGN KEY (ROL_ID) REFERENCES ROL(ROL_ID) -- Clave for�nea hacia ROL
);

-- ====================================================================================
-- Procedimiento almacenado: sp_UsuarioRol_Insertar
-- ====================================================================================
CREATE PROCEDURE sp_UsuarioRol_Insertar
    @USU_ID INT,
    @ROL_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO USUARIO_ROL (USU_ID, ROL_ID)
    VALUES (@USU_ID, @ROL_ID);
END
GO

-- ====================================================================================
-- Procedimiento almacenado: sp_UsuarioRol_ObtenerPorUsuario
-- ====================================================================================
CREATE PROCEDURE sp_UsuarioRol_ObtenerPorUsuario
    @USU_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM USUARIO_ROL WHERE USU_ID = @USU_ID;
END
GO

-- ====================================================================================
-- Procedimiento almacenado: sp_UsuarioRol_Eliminar
-- ====================================================================================
CREATE PROCEDURE sp_UsuarioRol_Eliminar
    @USU_ID INT,
    @ROL_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM USUARIO_ROL WHERE USU_ID = @USU_ID AND ROL_ID = @ROL_ID;
END
GO

-- ====================================================================================
-- Scripts de prueba de USUARIO_ROL
-- ====================================================================================

-- Declaraci�n de variables para pruebas
DECLARE @NuevoUsu_ID INT = 2;
DECLARE @NuevoRol_ID INT = 2;

-- *** Validaci�n inicial: Mostrar todas las relaciones existentes ***
PRINT '*** Validaci�n inicial: Mostrar todas las relaciones existentes ***';
EXEC sp_UsuarioRol_ObtenerPorUsuario @USU_ID = @NuevoUsu_ID;

-- *** Prueba: Insertar un nuevo rol para el usuario ***
PRINT '*** Prueba: Insertar un nuevo rol para el usuario ***';
EXEC sp_UsuarioRol_Insertar @USU_ID = @NuevoUsu_ID, @ROL_ID = @NuevoRol_ID;

-- *** Validaci�n: Mostrar roles despu�s de la inserci�n ***
PRINT '*** Validaci�n: Mostrar roles despu�s de la inserci�n ***';
EXEC sp_UsuarioRol_ObtenerPorUsuario @USU_ID = @NuevoUsu_ID;

-- *** Prueba: Eliminar el rol del usuario ***
PRINT '*** Prueba: Eliminar el rol del usuario ***';
EXEC sp_UsuarioRol_Eliminar @USU_ID = @NuevoUsu_ID, @ROL_ID = @NuevoRol_ID;

-- *** Validaci�n: Intentar obtener el rol eliminado ***
PRINT '*** Validaci�n: Intentar obtener el rol eliminado ***';
EXEC sp_UsuarioRol_ObtenerPorUsuario @USU_ID = @NuevoUsu_ID;

-- *** Validaci�n final: Mostrar todas las relaciones restantes ***
PRINT '*** Validaci�n final: Mostrar todas las relaciones restantes ***';
EXEC sp_UsuarioRol_ObtenerPorUsuario @USU_ID = @NuevoUsu_ID;

