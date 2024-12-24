-- Tabla de estados de inmueble
CREATE TABLE INMUEBLE_ESTADO (
    INM_EST_ID INT IDENTITY(1,1) PRIMARY KEY,
    INM_EST_DESCRIPCION NVARCHAR(50) NOT NULL UNIQUE
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla INMUEBLE_ESTADO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Estado_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO INMUEBLE_ESTADO (INM_EST_DESCRIPCION)
    VALUES (@Descripcion);

   	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla INMUEBLE_ESTADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de INMUEBLE_ESTADO.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_ESTADO_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM INMUEBLE_ESTADO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de INMUEBLE_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado del inmueble.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Estado_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM INMUEBLE_ESTADO WHERE INM_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en INMUEBLE_ESTADO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Estado_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE INMUEBLE_ESTADO
    SET INM_EST_DESCRIPCION = @Descripcion
    WHERE INM_EST_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de INMUEBLE_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado del inmueble.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Inmueble_Estado_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM INMUEBLE_ESTADO WHERE INM_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de pruebas para INMUEBLE_ESTADO
-- ====================================================================================

-- Declaración de variables para pruebas
DECLARE @InmuebleEstadoId INT;

-- *** Validación inicial: Mostrar todos los registros existentes ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes ***';
EXEC sp_Inmueble_ESTADO_ObtenerTodos;

-- *** Prueba: Insertar un nuevo estado de inmueble ***
PRINT '*** Prueba: Insertar un nuevo estado de inmueble ***';
EXEC sp_Inmueble_Estado_Insertar
    @Descripcion = 'Estado Inicial',
    @NuevoId = @InmuebleEstadoId OUTPUT;

PRINT 'ID del nuevo estado de inmueble insertado: ' + CAST(@InmuebleEstadoId AS NVARCHAR);

-- *** Validación: Mostrar el registro recién insertado por ID ***
PRINT '*** Validación: Mostrar el registro recién insertado por ID ***';
EXEC sp_Inmueble_Estado_ObtenerPorId @Id = @InmuebleEstadoId;

-- *** Prueba: Actualizar el estado del inmueble ***
PRINT '*** Prueba: Actualizar el estado del inmueble ***';
EXEC sp_Inmueble_Estado_Actualizar
    @Id = @InmuebleEstadoId,
    @Descripcion = 'Estado Actualizado';

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Inmueble_Estado_ObtenerPorId @Id = @InmuebleEstadoId;

-- *** Prueba: Eliminar el estado del inmueble ***
PRINT '*** Prueba: Eliminar el estado del inmueble ***';
EXEC sp_Inmueble_Estado_Eliminar @Id = @InmuebleEstadoId;

-- *** Validación: Intentar obtener el estado eliminado ***
PRINT '*** Validación: Intentar obtener el estado eliminado ***';
EXEC sp_Inmueble_Estado_ObtenerPorId @Id = @InmuebleEstadoId;

-- *** Validación final: Mostrar todos los registros restantes ***
PRINT '*** Validación final: Mostrar todos los registros restantes ***';
EXEC sp_Inmueble_ESTADO_ObtenerTodos;
