-- Tabla Inquilinos
CREATE TABLE INQUILINO (
    INQ_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    INM_ID INT NOT NULL FOREIGN KEY REFERENCES INMUEBLE(INM_ID),
    ADM_ID INT NOT NULL FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Inserta un nuevo registro en la tabla INQUILINO.
-- Entradas: @PER_ID INT, @INM_ID INT, @ADM_ID INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_Insertar
    @PER_ID INT,
    @INM_ID INT,
    @ADM_ID INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO INQUILINO (PER_ID, INM_ID, ADM_ID)
    VALUES (@PER_ID, @INM_ID, @ADM_ID);
	SET @NuevoId = SCOPE_IDENTITY();

END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve todos los registros de la tabla INQUILINO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de la tabla.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM INQUILINO;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve un registro de la tabla INQUILINO por su ID.
-- Entradas: @INQ_ID INT.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_ObtenerPorId
    @INQ_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM INQUILINO WHERE INQ_ID = @INQ_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Actualiza un registro en la tabla INQUILINO.
-- Entradas: @INQ_ID INT, @PER_ID INT, @INM_ID INT, @ADM_ID INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_Actualizar
    @INQ_ID INT,
    @PER_ID INT,
    @INM_ID INT,
    @ADM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE INQUILINO
    SET PER_ID = @PER_ID, INM_ID = @INM_ID, ADM_ID = @ADM_ID
    WHERE INQ_ID = @INQ_ID;
    SELECT @INQ_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Elimina un registro de la tabla INQUILINO por su ID.
-- Entradas: @INQ_ID INT.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Inquilino_Eliminar
    @INQ_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM INQUILINO WHERE INQ_ID = @INQ_ID;
END
GO

-- ====================================================================================
-- Script prueba de inquilino
-- ====================================================================================

-- Declaración de variables para pruebas
DECLARE @InquilinoId INT;
DECLARE @PER_ID INT = 1; -- Ejemplo de valor para persona
DECLARE @INM_ID INT = 1; -- Ejemplo de valor para inmueble
DECLARE @ADM_ID INT = 1; -- Ejemplo de valor para administrador

-- *** Validación inicial: Mostrar todos los registros existentes ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes ***';
EXEC sp_Inquilino_ObtenerTodos;

-- *** Prueba: Insertar un nuevo inquilino ***
PRINT '*** Prueba: Insertar un nuevo inquilino ***';
EXEC sp_Inquilino_Insertar
    @PER_ID = @PER_ID,
    @INM_ID = @INM_ID,
    @ADM_ID = @ADM_ID,
    @NuevoId = @InquilinoId OUTPUT;

PRINT 'ID del nuevo inquilino insertado: ' + CAST(@InquilinoId AS NVARCHAR);

-- *** Validación: Mostrar el registro recién insertado por ID ***
PRINT '*** Validación: Mostrar el registro recién insertado por ID ***';
EXEC sp_Inquilino_ObtenerPorId @INQ_ID = @InquilinoId;

-- *** Prueba: Actualizar el inquilino ***
PRINT '*** Prueba: Actualizar el inquilino ***';
EXEC sp_Inquilino_Actualizar
    @INQ_ID = @InquilinoId,
    @PER_ID = @PER_ID,
    @INM_ID = @INM_ID,
    @ADM_ID = @ADM_ID;

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Inquilino_ObtenerPorId @INQ_ID = @InquilinoId;

-- *** Prueba: Eliminar el inquilino ***
PRINT '*** Prueba: Eliminar el inquilino ***';
EXEC sp_Inquilino_Eliminar @INQ_ID = @InquilinoId;

-- *** Validación: Intentar obtener el inquilino eliminado ***
PRINT '*** Validación: Intentar obtener el inquilino eliminado ***';
EXEC sp_Inquilino_ObtenerPorId @INQ_ID = @InquilinoId;

-- *** Validación final: Mostrar todos los registros restantes ***
PRINT '*** Validación final: Mostrar todos los registros restantes ***';
EXEC sp_Inquilino_ObtenerTodos;
