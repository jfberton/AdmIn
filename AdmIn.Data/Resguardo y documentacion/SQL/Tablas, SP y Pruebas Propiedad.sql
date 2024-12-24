-- Tabla Propiedades
CREATE TABLE PROPIEDAD (
    PRO_ID INT IDENTITY(1,1) PRIMARY KEY,
    PRO_DESCRIPCION NVARCHAR(200) NOT NULL,
    DIR_ID INT NOT NULL FOREIGN KEY REFERENCES DIRECCION(DIR_ID),
    TEL_ID INT FOREIGN KEY REFERENCES TELEFONO(TEL_ID) NULL,
    PRO_UNIDADES INT,
    PRO_UNIDADES_ALQUILADAS INT,
    PRO_COMENTARIO TEXT,
    PRO_VALOR DECIMAL(18,2),
    PRO_SUPERFICIEM DECIMAL(18,2),
    ADM_ID INT FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID),
    CLI_ID INT NOT NULL FOREIGN KEY REFERENCES CLIENTE(CLI_ID),
    PRO_TIP_ID INT NOT NULL FOREIGN KEY REFERENCES PROPIEDAD_TIPO(PRO_TIP_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PROPIEDAD.
-- Entradas: @ClienteId INT, @TipoId INT, @Descripcion NVARCHAR(200).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Insertar
    @ClienteId INT,
    @TipoId INT,
    @Descripcion NVARCHAR(200),
    @DireccionId INT,
    @NuevoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PROPIEDAD (CLI_ID, PRO_TIP_ID, PRO_DESCRIPCION, DIR_ID)
    VALUES (@ClienteId, @TipoId, @Descripcion, @DireccionId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO


-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PROPIEDAD.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PROPIEDAD.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PROPIEDAD;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PROPIEDAD basado en su ID.
-- Entradas: @Id INT - ID de la propiedad.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PROPIEDAD WHERE PRO_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PROPIEDAD basado en su ID.
-- Entradas: @Id INT, @ClienteId INT, @TipoId INT, @Descripcion NVARCHAR(200).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Actualizar
    @Id INT,
    @ClienteId INT,
    @TipoId INT,
	@DireccionId INT,
    @Descripcion NVARCHAR(200)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PROPIEDAD
    SET CLI_ID = @ClienteId,
        PRO_TIP_ID = @TipoId,
		DIR_ID = @DireccionId,
        PRO_DESCRIPCION = @Descripcion
    WHERE PRO_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PROPIEDAD basado en su ID.
-- Entradas: @Id INT - ID de la propiedad.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PROPIEDAD WHERE PRO_ID = @Id;
END;
GO

-- =============================================
-- Script de Pruebas: PROPIEDAD
-- =============================================

-- Declaración de variables para pruebas
DECLARE @PropiedadId INT;
DECLARE @ClienteId INT = 1; -- Ajustar según un cliente existente
DECLARE @TipoId INT = 1; -- Ajustar según un tipo de propiedad existente
DECLARE @DireccionId INT = 1; -- Ajustar según una dirección existente
DECLARE @Descripcion NVARCHAR(200) = 'Propiedad de prueba';

-- *** Validación inicial: Mostrar todos los registros existentes en PROPIEDAD ***
PRINT '*** Validación inicial: Mostrar todos los registros existentes en PROPIEDAD ***';
EXEC sp_Propiedad_ObtenerTodos;

-- *** Prueba: Insertar un nuevo registro en PROPIEDAD ***
PRINT '*** Prueba: Insertar un nuevo registro en PROPIEDAD ***';
EXEC sp_Propiedad_Insertar
    @ClienteId = @ClienteId,
    @TipoId = @TipoId,
    @Descripcion = @Descripcion,
    @DireccionId = @DireccionId,
    @NuevoId = @PropiedadId OUTPUT;

PRINT 'ID de la nueva propiedad insertada: ' + CAST(@PropiedadId AS NVARCHAR);

-- *** Validación: Mostrar todos los registros después de la inserción ***
PRINT '*** Validación: Mostrar todos los registros después de la inserción ***';
EXEC sp_Propiedad_ObtenerTodos;

-- *** Prueba: Obtener registro de PROPIEDAD por ID ***
PRINT '*** Prueba: Obtener registro de PROPIEDAD por ID ***';
EXEC sp_Propiedad_ObtenerPorId @Id = @PropiedadId;

-- *** Prueba: Actualizar un registro en PROPIEDAD ***
PRINT '*** Prueba: Actualizar un registro en PROPIEDAD ***';
EXEC sp_Propiedad_Actualizar
    @Id = @PropiedadId,
    @ClienteId = @ClienteId,
    @TipoId = @TipoId,
    @DireccionId = @DireccionId,
    @Descripcion = 'Propiedad actualizada';

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Propiedad_ObtenerPorId @Id = @PropiedadId;

-- *** Prueba: Eliminar un registro en PROPIEDAD ***
PRINT '*** Prueba: Eliminar un registro en PROPIEDAD ***';
EXEC sp_Propiedad_Eliminar @Id = @PropiedadId;

-- *** Validación: Mostrar todos los registros después de la eliminación ***
PRINT '*** Validación: Mostrar todos los registros después de la eliminación ***';
EXEC sp_Propiedad_ObtenerTodos;

-- *** Validación final: Confirmar que no quedan registros pendientes ***
PRINT '*** Validación final: Confirmar que no quedan registros pendientes ***';

