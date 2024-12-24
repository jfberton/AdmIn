-- Tabla de estados de contrato
CREATE TABLE CONTRATO_ESTADO (
    CON_EST_ID INT IDENTITY(1,1) PRIMARY KEY,
    CON_EST_DESCRIPCION NVARCHAR(50) NOT NULL UNIQUE
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla CONTRATO_ESTADO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO CONTRATO_ESTADO (CON_EST_DESCRIPCION)
    VALUES (@Descripcion);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla CONTRATO_ESTADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de CONTRATO_ESTADO.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CONTRATO_ESTADO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de CONTRATO_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado del contrato.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CONTRATO_ESTADO WHERE CON_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en CONTRATO_ESTADO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE CONTRATO_ESTADO
    SET CON_EST_DESCRIPCION = @Descripcion
    WHERE CON_EST_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de CONTRATO_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado del contrato.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Contrato_Estado_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM CONTRATO_ESTADO WHERE CON_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de pruebas de CONTRATO_ESTADO
-- ====================================================================================

-- Declaración de variables para pruebas
DECLARE @EstadoId INT;
DECLARE @Descripcion NVARCHAR(100) = 'Estado de Prueba';
DECLARE @NuevaDescripcion NVARCHAR(100) = 'Estado Actualizado';

-- *** Validación inicial: Mostrar todos los registros existentes ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes ***';
EXEC sp_Contrato_Estado_ObtenerTodos;

-- *** Prueba: Insertar un nuevo estado de contrato ***
PRINT '*** Prueba: Insertar un nuevo estado de contrato ***';
EXEC sp_Contrato_Estado_Insertar
    @Descripcion = @Descripcion,
    @NuevoId = @EstadoId OUTPUT;

PRINT 'ID del nuevo estado insertado: ' + CAST(@EstadoId AS NVARCHAR);

-- *** Validación: Mostrar el registro recién insertado por ID ***
PRINT '*** Validación: Mostrar el registro recién insertado por ID ***';
EXEC sp_Contrato_Estado_ObtenerPorId @Id = @EstadoId;

-- *** Prueba: Actualizar el estado de contrato ***
PRINT '*** Prueba: Actualizar el estado de contrato ***';
EXEC sp_Contrato_Estado_Actualizar
    @Id = @EstadoId,
    @Descripcion = @NuevaDescripcion;

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Contrato_Estado_ObtenerPorId @Id = @EstadoId;

-- *** Prueba: Eliminar el estado de contrato ***
PRINT '*** Prueba: Eliminar el estado de contrato ***';
EXEC sp_Contrato_Estado_Eliminar @Id = @EstadoId;

-- *** Validación: Intentar obtener el estado eliminado ***
PRINT '*** Validación: Intentar obtener el estado eliminado ***';
EXEC sp_Contrato_Estado_ObtenerPorId @Id = @EstadoId;

-- *** Validación final: Mostrar todos los registros restantes ***
PRINT '*** Validación final: Mostrar todos los registros restantes ***';
EXEC sp_Contrato_Estado_ObtenerTodos;
