-- Tabla de estados de pago
CREATE TABLE PAGO_ESTADO (
    PAGO_EST_ID INT IDENTITY(1,1) PRIMARY KEY,
    PAGO_EST_DESCRIPCION NVARCHAR(50) NOT NULL UNIQUE
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PAGO_ESTADO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_Insertar
    @PAGO_EST_DESCRIPCION NVARCHAR(50),
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PAGO_ESTADO (PAGO_EST_DESCRIPCION)
    VALUES (@PAGO_EST_DESCRIPCION);
	SET @NuevoId = SCOPE_IDENTITY();

END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve todos los registros de la tabla PAGO_ESTADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PAGO_ESTADO.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PAGO_ESTADO;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve un registro específico de PAGO_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado de pago.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_ObtenerPorId
    @PAGO_EST_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM PAGO_ESTADO WHERE PAGO_EST_ID = @PAGO_EST_ID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Actualiza un registro en PAGO_ESTADO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_Actualizar
    @PAGO_EST_ID INT,
    @PAGO_EST_DESCRIPCION NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PAGO_ESTADO
    SET PAGO_EST_DESCRIPCION = @PAGO_EST_DESCRIPCION
    WHERE PAGO_EST_ID = @PAGO_EST_ID;
    SELECT @PAGO_EST_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Elimina un registro de PAGO_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado de pago.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Pago_Estado_Eliminar
    @PAGO_EST_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PAGO_ESTADO WHERE PAGO_EST_ID = @PAGO_EST_ID;
END
GO

-- ====================================================================================
-- Scripts de prueba de PAGO_ESTADO
-- ====================================================================================

-- Declaración de variables para pruebas
DECLARE @EstadoId INT;
DECLARE @Descripcion NVARCHAR(50) = 'Estado Inicial de Prueba';
DECLARE @NuevaDescripcion NVARCHAR(50) = 'Estado Actualizado de Prueba';

-- *** Validación inicial: Mostrar todos los registros existentes ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes ***';
EXEC sp_Pago_Estado_ObtenerTodos;

-- *** Prueba: Insertar un nuevo estado de pago ***
PRINT '*** Prueba: Insertar un nuevo estado de pago ***';
EXEC sp_Pago_Estado_Insertar
    @PAGO_EST_DESCRIPCION = @Descripcion,
    @NuevoId = @EstadoId OUTPUT;

PRINT 'ID del nuevo estado insertado: ' + CAST(@EstadoId AS NVARCHAR);

-- *** Validación: Mostrar el registro recién insertado por ID ***
PRINT '*** Validación: Mostrar el registro recién insertado por ID ***';
EXEC sp_Pago_Estado_ObtenerPorId @PAGO_EST_ID = @EstadoId;

-- *** Prueba: Actualizar el estado de pago ***
PRINT '*** Prueba: Actualizar el estado de pago ***';
EXEC sp_Pago_Estado_Actualizar
    @PAGO_EST_ID = @EstadoId,
    @PAGO_EST_DESCRIPCION = @NuevaDescripcion;

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Pago_Estado_ObtenerPorId @PAGO_EST_ID = @EstadoId;

-- *** Prueba: Eliminar el estado de pago ***
PRINT '*** Prueba: Eliminar el estado de pago ***';
EXEC sp_Pago_Estado_Eliminar @PAGO_EST_ID = @EstadoId;

-- *** Validación: Intentar obtener el estado eliminado ***
PRINT '*** Validación: Intentar obtener el estado eliminado ***';
EXEC sp_Pago_Estado_ObtenerPorId @PAGO_EST_ID = @EstadoId;

-- *** Validación final: Mostrar todos los registros restantes ***
PRINT '*** Validación final: Mostrar todos los registros restantes ***';
EXEC sp_Pago_Estado_ObtenerTodos;

