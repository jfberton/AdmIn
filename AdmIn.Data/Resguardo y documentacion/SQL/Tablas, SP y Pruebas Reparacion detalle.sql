-- Tabla Detalle de Reparaciones
CREATE TABLE REPARACION_DETALLE (
    REP_DET_ID INT IDENTITY(1,1) PRIMARY KEY,
    REP_ID INT NOT NULL FOREIGN KEY REFERENCES REPARACION(REP_ID),
    REP_DET_DESCRIPCION NVARCHAR(500),
    REP_DET_COSTO DECIMAL(18,2),
    REP_DET_FECHA DATE
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION_DETALLE.
-- Entradas: @ReparacionId INT, @Descripcion NVARCHAR(200), @Costo DECIMAL(18,2).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_Insertar
    @ReparacionId INT,
    @Descripcion NVARCHAR(200),
    @Costo DECIMAL(18,2),
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO REPARACION_DETALLE (REP_ID, REP_DET_DESCRIPCION, REP_DET_COSTO)
    VALUES (@ReparacionId, @Descripcion, @Costo);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION_DETALLE.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION_DETALLE.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM REPARACION_DETALLE;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de REPARACION_DETALLE basado en su ID.
-- Entradas: @Id INT - ID del detalle de reparación.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_ObtenerPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM REPARACION_DETALLE WHERE REP_DET_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en REPARACION_DETALLE basado en su ID.
-- Entradas: @Id INT, @ReparacionId INT, @Descripcion NVARCHAR(200), @Costo DECIMAL(18,2).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_Actualizar
    @Id INT,
    @ReparacionId INT,
    @Descripcion NVARCHAR(200),
    @Costo DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE REPARACION_DETALLE
    SET REP_ID = @ReparacionId,
        REP_DET_DESCRIPCION = @Descripcion,
        REP_DET_COSTO = @Costo
    WHERE REP_DET_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de REPARACION_DETALLE basado en su ID.
-- Entradas: @Id INT - ID del detalle de reparación.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Detalle_Eliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM REPARACION_DETALLE WHERE REP_DET_ID = @Id;
END;
GO

-- ====================================================================================
-- Scripts de prueba de REPARACION_DETALLE
-- ====================================================================================

-- Prueba: Insertar un registro en REPARACION_DETALLE
DECLARE @NuevoId INT;
EXEC sp_Reparacion_Detalle_Insertar
    @ReparacionId = 1, -- ID de una reparación preexistente o nueva
    @Descripcion = 'Sustitución de piezas en la cocina',
    @Costo = 300.00,
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo registro insertado con ID: ' + CAST(@NuevoId AS NVARCHAR);

-- Prueba: Obtener todos los registros de REPARACION_DETALLE
EXEC sp_Reparacion_Detalle_ObtenerTodos;

-- Prueba: Obtener un registro específico por ID
EXEC sp_Reparacion_Detalle_ObtenerPorId
    @Id = @NuevoId; -- ID del registro insertado anteriormente

-- Prueba: Actualizar un registro en REPARACION_DETALLE
EXEC sp_Reparacion_Detalle_Actualizar
    @Id = @NuevoId, -- ID del registro a actualizar
    @ReparacionId = 1, -- ID de la reparación
    @Descripcion = 'Sustitución de piezas en el baño',
    @Costo = 350.00;

-- Verificación final: Obtener el registro actualizado
EXEC sp_Reparacion_Detalle_ObtenerPorId
    @Id = @NuevoId;

-- Prueba: Eliminar un registro de REPARACION_DETALLE
EXEC sp_Reparacion_Detalle_Eliminar
    @Id = @NuevoId; -- ID del registro a eliminar

-- Verificación final: Obtener todos los registros tras la eliminación
EXEC sp_Reparacion_Detalle_ObtenerTodos;
