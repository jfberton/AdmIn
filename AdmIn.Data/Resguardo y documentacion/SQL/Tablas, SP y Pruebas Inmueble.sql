-- Tabla Inmuebles
CREATE TABLE INMUEBLE (
    INM_ID INT IDENTITY(1,1) PRIMARY KEY,
    PRO_ID INT NOT NULL FOREIGN KEY REFERENCES PROPIEDAD(PRO_ID),
    DIR_ID INT NOT NULL FOREIGN KEY REFERENCES DIRECCION(DIR_ID),
    INM_DESCRIPCION NVARCHAR(200),
    INM_COMENTARIO TEXT,
    INM_VALOR DECIMAL(18,2),
    INM_SUPERFICIEM DECIMAL(18,2),
    INM_CONSTRUIDOM DECIMAL(18,2),
    TEL_ID INT FOREIGN KEY REFERENCES TELEFONO(TEL_ID) NULL,
    INM_ESTADO_ID INT FOREIGN KEY REFERENCES INMUEBLE_ESTADO(INM_EST_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Inserta un nuevo registro en la tabla INMUEBLE.
-- Entradas: @PRO_ID INT, @DIR_ID INT, @INM_DESCRIPCION NVARCHAR(200), @INM_COMENTARIO TEXT, @INM_VALOR DECIMAL(18,2), 
--          @INM_SUPERFICIEM DECIMAL(18,2), @INM_CONSTRUIDOM DECIMAL(18,2), @TEL_ID INT, @INM_ESTADO_ID INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Insertar
    @PRO_ID INT,
    @DIR_ID INT,
    @INM_DESCRIPCION NVARCHAR(200) = NULL,
    @INM_COMENTARIO TEXT = NULL,
    @INM_VALOR DECIMAL(18,2) = NULL,
    @INM_SUPERFICIEM DECIMAL(18,2) = NULL,
    @INM_CONSTRUIDOM DECIMAL(18,2) = NULL,
    @TEL_ID INT = NULL,
    @INM_ESTADO_ID INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO INMUEBLE (PRO_ID, DIR_ID, INM_DESCRIPCION, INM_COMENTARIO, INM_VALOR, INM_SUPERFICIEM, INM_CONSTRUIDOM, TEL_ID, INM_ESTADO_ID)
    VALUES (@PRO_ID, @DIR_ID, @INM_DESCRIPCION, @INM_COMENTARIO, @INM_VALOR, @INM_SUPERFICIEM, @INM_CONSTRUIDOM, @TEL_ID, @INM_ESTADO_ID);
    
	SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve todos los registros de la tabla INMUEBLE.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de la tabla.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM INMUEBLE;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Devuelve un registro de la tabla INMUEBLE por su ID.
-- Entradas: @INM_ID INT.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_ObtenerPorId
    @INM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM INMUEBLE WHERE INM_ID = @INM_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Actualiza un registro en la tabla INMUEBLE.
-- Entradas: @INM_ID INT, @PRO_ID INT, @DIR_ID INT, @INM_DESCRIPCION NVARCHAR(200), @INM_COMENTARIO TEXT, @INM_VALOR DECIMAL(18,2), 
--          @INM_SUPERFICIEM DECIMAL(18,2), @INM_CONSTRUIDOM DECIMAL(18,2), @TEL_ID INT, @INM_ESTADO_ID INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Actualizar
    @INM_ID INT,
    @PRO_ID INT,
    @DIR_ID INT,
    @INM_DESCRIPCION NVARCHAR(200) = NULL,
    @INM_COMENTARIO TEXT = NULL,
    @INM_VALOR DECIMAL(18,2) = NULL,
    @INM_SUPERFICIEM DECIMAL(18,2) = NULL,
    @INM_CONSTRUIDOM DECIMAL(18,2) = NULL,
    @TEL_ID INT = NULL,
    @INM_ESTADO_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE INMUEBLE
    SET PRO_ID = @PRO_ID, DIR_ID = @DIR_ID, INM_DESCRIPCION = @INM_DESCRIPCION, INM_COMENTARIO = @INM_COMENTARIO, 
        INM_VALOR = @INM_VALOR, INM_SUPERFICIEM = @INM_SUPERFICIEM, INM_CONSTRUIDOM = @INM_CONSTRUIDOM, 
        TEL_ID = @TEL_ID, INM_ESTADO_ID = @INM_ESTADO_ID
    WHERE INM_ID = @INM_ID;
    SELECT @INM_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 13/12/2024
-- Descripción: Elimina un registro de la tabla INMUEBLE por su ID.
-- Entradas: @INM_ID INT.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Eliminar
    @INM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM INMUEBLE WHERE INM_ID = @INM_ID;
END
GO

-- ====================================================================================
-- Script de pruebas de INMUEBLE
-- ====================================================================================

-- Declaración de variables para pruebas
DECLARE @InmuebleId INT;
DECLARE @PRO_ID INT = 6; -- Ejemplo de valor para la propiedad
DECLARE @DIR_ID INT = 2; -- Ejemplo de valor para la dirección
DECLARE @INM_ESTADO_ID INT = 1; -- Ejemplo de estado

-- *** Validación inicial: Mostrar todos los registros existentes ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes ***';
EXEC sp_Inmueble_ObtenerTodos;

-- *** Prueba: Insertar un nuevo inmueble ***
PRINT '*** Prueba: Insertar un nuevo inmueble ***';
EXEC sp_Inmueble_Insertar
    @PRO_ID = @PRO_ID,
    @DIR_ID = @DIR_ID,
    @INM_DESCRIPCION = 'Inmueble de prueba',
    @INM_COMENTARIO = 'Comentario de prueba',
    @INM_VALOR = 100000.00,
    @INM_SUPERFICIEM = 120.50,
    @INM_CONSTRUIDOM = 100.00,
    @TEL_ID = NULL,
    @INM_ESTADO_ID = @INM_ESTADO_ID,
    @NuevoId = @InmuebleId OUTPUT;

PRINT 'ID del nuevo inmueble insertado: ' + CAST(@InmuebleId AS NVARCHAR);

-- *** Validación: Mostrar el registro recién insertado por ID ***
PRINT '*** Validación: Mostrar el registro recién insertado por ID ***';
EXEC sp_Inmueble_ObtenerPorId @INM_ID = @InmuebleId;

-- *** Prueba: Actualizar el inmueble ***
PRINT '*** Prueba: Actualizar el inmueble ***';
EXEC sp_Inmueble_Actualizar
    @INM_ID = @InmuebleId,
    @PRO_ID = @PRO_ID,
    @DIR_ID = @DIR_ID,
    @INM_DESCRIPCION = 'Inmueble actualizado',
    @INM_COMENTARIO = 'Comentario actualizado',
    @INM_VALOR = 120000.00,
    @INM_SUPERFICIEM = 130.00,
    @INM_CONSTRUIDOM = 120.00,
    @TEL_ID = NULL,
    @INM_ESTADO_ID = @INM_ESTADO_ID;

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Inmueble_ObtenerPorId @INM_ID = @InmuebleId;

-- *** Prueba: Eliminar el inmueble ***
PRINT '*** Prueba: Eliminar el inmueble ***';
EXEC sp_Inmueble_Eliminar @INM_ID = @InmuebleId;

-- *** Validación: Intentar obtener el inmueble eliminado ***
PRINT '*** Validación: Intentar obtener el inmueble eliminado ***';
EXEC sp_Inmueble_ObtenerPorId @INM_ID = @InmuebleId;

-- *** Validación final: Mostrar todos los registros restantes ***
PRINT '*** Validación final: Mostrar todos los registros restantes ***';
EXEC sp_Inmueble_ObtenerTodos;
