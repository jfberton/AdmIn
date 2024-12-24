-- Tabla Detalle de Reparaciones relacionadas con Propiedad
CREATE TABLE REPARACION_PROPIEDAD (
	REP_PRO_ID INT IDENTITY(1,1) PRIMARY KEY,
	PRO_ID INT NOT NULL FOREIGN KEY REFERENCES PROPIEDAD(PRO_ID),
	REP_ID INT NOT NULL FOREIGN KEY REFERENCES REPARACION(REP_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION_PROPIEDAD.
-- Entradas: @PropiedadId INT, @ReparacionId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_Insertar
    @PropiedadId INT,
    @ReparacionId INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO REPARACION_PROPIEDAD (PRO_ID, REP_ID)
    VALUES (@PropiedadId, @ReparacionId);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION_PROPIEDAD.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION_PROPIEDAD.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_PROPIEDAD;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de REPARACION_PROPIEDAD basado en su ID.
-- Entradas: @Id INT - ID del registro de reparación-propiedad.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM REPARACION_PROPIEDAD WHERE REP_PRO_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en REPARACION_PROPIEDAD basado en su ID.
-- Entradas: @Id INT, @PropiedadId INT, @ReparacionId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_Actualizar
    @Id INT,
    @PropiedadId INT,
    @ReparacionId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE REPARACION_PROPIEDAD
    SET PRO_ID = @PropiedadId,
        REP_ID = @ReparacionId
    WHERE REP_PRO_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de REPARACION_PROPIEDAD basado en su ID.
-- Entradas: @Id INT - ID del registro de reparación-propiedad.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Propiedad_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM REPARACION_PROPIEDAD WHERE REP_PRO_ID = @Id;
END;
GO

-- ====================================================================================
-- Scripts de prueba de REPARACION_PROPIEDAD
-- ====================================================================================

-- Prueba: Insertar un registro en REPARACION_PROPIEDAD
DECLARE @NuevoId INT;
EXEC sp_Reparacion_Propiedad_Insertar
    @PropiedadId = 1, -- ID de una propiedad preexistente
    @ReparacionId = 1, -- ID de una reparación preexistente
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo registro de reparación-propiedad insertado con ID: ' + CAST(@NuevoId AS NVARCHAR);

-- Prueba: Obtener todos los registros de REPARACION_PROPIEDAD
EXEC sp_Reparacion_Propiedad_ObtenerTodos;

-- Prueba: Obtener un registro específico por ID
EXEC sp_Reparacion_Propiedad_ObtenerPorId
    @Id = @NuevoId; -- ID del registro insertado anteriormente

-- Prueba: Actualizar un registro en REPARACION_PROPIEDAD
EXEC sp_Reparacion_Propiedad_Actualizar
    @Id = @NuevoId, -- ID del registro a actualizar
    @PropiedadId = 1, -- ID de la propiedad a asociar
    @ReparacionId = 2; -- ID de la reparación a asociar

-- Verificación final: Obtener el registro actualizado
EXEC sp_Reparacion_Propiedad_ObtenerPorId
    @Id = @NuevoId;

-- Prueba: Eliminar un registro de REPARACION_PROPIEDAD
EXEC sp_Reparacion_Propiedad_Eliminar
    @Id = @NuevoId; -- ID del registro a eliminar

-- Verificación final: Obtener todos los registros tras la eliminación
EXEC sp_Reparacion_Propiedad_ObtenerTodos;

