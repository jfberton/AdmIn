-- Tabla Reparaciones
CREATE TABLE REPARACION (
    REP_ID INT IDENTITY(1,1) PRIMARY KEY,
    REP_FECHA_SOLICITUD DATE NOT NULL,
    REP_FECHA_INICIO DATE,
    REP_FECHA_FIN DATE,
    REP_COSTO_ESTIMADO DECIMAL(18,2),
    REP_COSTO_FINAL DECIMAL(18,2),
    REP_DESCRIPCION NVARCHAR(500),
    REP_EST_ID INT NOT NULL FOREIGN KEY REFERENCES REPARACION_ESTADO(REP_EST_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Inserta un nuevo registro en la tabla REPARACION.
-- Entradas: @EstadoId INT, @Descripcion NVARCHAR(200), @Fecha DATE.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Insertar
    @REP_FECHA_SOLICITUD DATE,
    @REP_FECHA_INICIO DATE = NULL,
    @REP_FECHA_FIN DATE = NULL,
    @REP_COSTO_ESTIMADO DECIMAL(18,2) = NULL,
    @REP_COSTO_FINAL DECIMAL(18,2) = NULL,
    @REP_DESCRIPCION NVARCHAR(500) = NULL,
    @REP_EST_ID INT,
	@NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO REPARACION (REP_FECHA_SOLICITUD, REP_FECHA_INICIO, REP_FECHA_FIN, REP_COSTO_ESTIMADO, REP_COSTO_FINAL, REP_DESCRIPCION, REP_EST_ID)
    VALUES (@REP_FECHA_SOLICITUD, @REP_FECHA_INICIO, @REP_FECHA_FIN, @REP_COSTO_ESTIMADO, @REP_COSTO_FINAL, @REP_DESCRIPCION, @REP_EST_ID);
	SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve todos los registros de la tabla REPARACION.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de REPARACION.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM REPARACION;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Devuelve un registro específico de REPARACION basado en su ID.
-- Entradas: @Id INT - ID de la reparación.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_ObtenerPorId
    @REP_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM REPARACION WHERE REP_ID = @REP_ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Actualiza un registro en REPARACION basado en su ID.
-- Entradas: @Id INT, @EstadoId INT, @Descripcion NVARCHAR(200), @Fecha DATE.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Actualizar
    @REP_ID INT,
    @REP_FECHA_SOLICITUD DATE,
    @REP_FECHA_INICIO DATE = NULL,
    @REP_FECHA_FIN DATE = NULL,
    @REP_COSTO_ESTIMADO DECIMAL(18,2) = NULL,
    @REP_COSTO_FINAL DECIMAL(18,2) = NULL,
    @REP_DESCRIPCION NVARCHAR(500) = NULL,
    @REP_EST_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE REPARACION
    SET REP_FECHA_SOLICITUD = @REP_FECHA_SOLICITUD,
        REP_FECHA_INICIO = @REP_FECHA_INICIO,
        REP_FECHA_FIN = @REP_FECHA_FIN,
        REP_COSTO_ESTIMADO = @REP_COSTO_ESTIMADO,
        REP_COSTO_FINAL = @REP_COSTO_FINAL,
        REP_DESCRIPCION = @REP_DESCRIPCION,
        REP_EST_ID = @REP_EST_ID
    WHERE REP_ID = @REP_ID;
    SELECT @REP_ID AS UpdatedID;
END
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 18/12/2024
-- Descripción: Elimina un registro de REPARACION basado en su ID.
-- Entradas: @Id INT - ID de la reparación.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Reparacion_Eliminar
    @REP_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM REPARACION WHERE REP_ID = @REP_ID;
END
GO

-- ====================================================================================
-- Scripts de prueba de REPARACION
-- ====================================================================================

-- Prueba: Insertar un registro en REPARACION
DECLARE @NuevoId INT;
EXEC sp_Reparacion_Insertar
    @REP_FECHA_SOLICITUD = '2024-12-18', -- Fecha de solicitud
    @REP_FECHA_INICIO = '2024-12-20', -- Fecha de inicio estimada
    @REP_EST_ID = 1, -- ID del estado (puede variar según los datos preexistentes)
    @REP_DESCRIPCION = 'Reparación de grifería en baño',
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo registro insertado con ID: ' + CAST(@NuevoId AS NVARCHAR);

-- Prueba: Obtener todos los registros de REPARACION
EXEC sp_Reparacion_ObtenerTodos;

-- Prueba: Obtener un registro específico por ID
EXEC sp_Reparacion_ObtenerPorId
    @REP_ID = @NuevoId; -- ID del registro insertado anteriormente

-- Prueba: Actualizar un registro en REPARACION
EXEC sp_Reparacion_Actualizar
    @REP_ID = @NuevoId, -- ID del registro a actualizar
    @REP_FECHA_SOLICITUD = '2024-12-19',
    @REP_FECHA_INICIO = '2024-12-21',
    @REP_FECHA_FIN = '2024-12-25',
    @REP_COSTO_ESTIMADO = 1500.00,
    @REP_COSTO_FINAL = 1600.00,
    @REP_DESCRIPCION = 'Reparación finalizada con costo actualizado',
    @REP_EST_ID = 2; -- Nuevo estado

-- Verificación final: Obtener el registro actualizado
EXEC sp_Reparacion_ObtenerPorId
    @REP_ID = @NuevoId;

-- Prueba: Eliminar un registro de REPARACION
EXEC sp_Reparacion_Eliminar
    @REP_ID = @NuevoId; -- ID del registro a eliminar

-- Verificación final: Obtener todos los registros tras la eliminación
EXEC sp_Reparacion_ObtenerTodos;
