-- Tabla Detalle de Reparaciones relacionadas con Inmueble
CREATE TABLE REPARACION_INMUEBLE (
	REP_INM_ID INT IDENTITY(1,1) PRIMARY KEY,
	INM_ID INT NOT NULL FOREIGN KEY REFERENCES INMUEBLE(INM_ID),
	REP_ID INT NOT NULL FOREIGN KEY REFERENCES REPARACION(REP_ID)
);


-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION_INMUEBLE.
-- Entradas: @InmuebleId INT, @ReparacionId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_Insertar
    @InmuebleId INT,
    @ReparacionId INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO REPARACION_INMUEBLE (INM_ID, REP_ID)
    VALUES (@InmuebleId, @ReparacionId);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION_INMUEBLE.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION_INMUEBLE.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_INMUEBLE;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de REPARACION_INMUEBLE basado en su ID.
-- Entradas: @Id INT - ID del registro de reparación-inmueble.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_INMUEBLE WHERE REP_INM_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en REPARACION_INMUEBLE basado en su ID.
-- Entradas: @Id INT, @InmuebleId INT, @ReparacionId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_Actualizar
    @Id INT,
    @InmuebleId INT,
    @ReparacionId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE REPARACION_INMUEBLE
    SET INM_ID = @InmuebleId,
        REP_ID = @ReparacionId
    WHERE REP_INM_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de REPARACION_INMUEBLE basado en su ID.
-- Entradas: @Id INT - ID del registro de reparación-inmueble.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Inmueble_Eliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM REPARACION_INMUEBLE WHERE REP_INM_ID = @Id;
END;
GO

-- ====================================================================================
-- Scripts de prueba de REPARACION_INMUEBLE
-- ====================================================================================

-- Prueba: Insertar un registro en REPARACION_INMUEBLE
DECLARE @NuevoId INT;
EXEC sp_Reparacion_Inmueble_Insertar
    @InmuebleId = 1, -- ID de un inmueble preexistente
    @ReparacionId = 1, -- ID de una reparación preexistente
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo registro de reparación-inmueble insertado con ID: ' + CAST(@NuevoId AS NVARCHAR);

-- Prueba: Obtener todos los registros de REPARACION_INMUEBLE
EXEC sp_Reparacion_Inmueble_ObtenerTodos;

-- Prueba: Obtener un registro específico por ID
EXEC sp_Reparacion_Inmueble_ObtenerPorId
    @Id = @NuevoId; -- ID del registro insertado anteriormente

-- Prueba: Actualizar un registro en REPARACION_INMUEBLE
EXEC sp_Reparacion_Inmueble_Actualizar
    @Id = @NuevoId, -- ID del registro a actualizar
    @InmuebleId = 1, -- ID del inmueble a asociar
    @ReparacionId = 2; -- ID de la reparación a asociar

-- Verificación final: Obtener el registro actualizado
EXEC sp_Reparacion_Inmueble_ObtenerPorId
    @Id = @NuevoId;

-- Prueba: Eliminar un registro de REPARACION_INMUEBLE
EXEC sp_Reparacion_Inmueble_Eliminar
    @Id = @NuevoId; -- ID del registro a eliminar

-- Verificación final: Obtener todos los registros tras la eliminación
EXEC sp_Reparacion_Inmueble_ObtenerTodos;
