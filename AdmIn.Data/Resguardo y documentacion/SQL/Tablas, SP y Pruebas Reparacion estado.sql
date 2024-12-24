-- Tabla Estado de Reparaciones
CREATE TABLE REPARACION_ESTADO (
    REP_EST_ID INT IDENTITY(1,1) PRIMARY KEY,
    REP_EST_DESCRIPCION NVARCHAR(50) NOT NULL UNIQUE
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION_ESTADO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO REPARACION_ESTADO (REP_EST_DESCRIPCION)
    VALUES (@Descripcion);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION_ESTADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION_ESTADO.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_ESTADO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de REPARACION_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado de reparación.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_ESTADO WHERE REP_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en REPARACION_ESTADO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE REPARACION_ESTADO
    SET REP_EST_DESCRIPCION = @Descripcion
    WHERE REP_EST_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de REPARACION_ESTADO basado en su ID.
-- Entradas: @Id INT - ID del estado de reparación.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Estado_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM REPARACION_ESTADO WHERE REP_EST_ID = @Id;
END;
GO

-- ====================================================================================
-- Scripts de prueba de REPARACION_ESTADO
-- ====================================================================================

-- Prueba: Insertar un registro en REPARACION_ESTADO
DECLARE @NuevoId INT;
EXEC sp_Reparacion_Estado_Insertar
    @Descripcion = 'En Progreso', -- Descripción del estado
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo registro insertado con ID: ' + CAST(@NuevoId AS NVARCHAR);

-- Prueba: Obtener todos los registros de REPARACION_ESTADO
EXEC sp_Reparacion_Estado_ObtenerTodos;

-- Prueba: Obtener un registro específico por ID
EXEC sp_Reparacion_Estado_ObtenerPorId
    @Id = @NuevoId; -- ID del registro insertado anteriormente

-- Prueba: Actualizar un registro en REPARACION_ESTADO
EXEC sp_Reparacion_Estado_Actualizar
    @Id = @NuevoId, -- ID del registro a actualizar
    @Descripcion = 'Completado'; -- Nueva descripción

-- Verificación final: Obtener el registro actualizado
EXEC sp_Reparacion_Estado_ObtenerPorId
    @Id = @NuevoId;

-- Prueba: Eliminar un registro de REPARACION_ESTADO
EXEC sp_Reparacion_Estado_Eliminar
    @Id = @NuevoId; -- ID del registro a eliminar

-- Verificación final: Obtener todos los registros tras la eliminación
EXEC sp_Reparacion_Estado_ObtenerTodos;

