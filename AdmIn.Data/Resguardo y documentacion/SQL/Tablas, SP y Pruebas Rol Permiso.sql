-- ====================================================================================
-- Tabla intermedia entre usuarios y permisos
-- ====================================================================================
CREATE TABLE ROL_PERMISO (
    ROL_ID INT NOT NULL,
    PERM_ID INT NOT NULL,
    PRIMARY KEY (ROL_ID, PERM_ID), -- Clave primaria compuesta
    FOREIGN KEY (ROL_ID) REFERENCES USUARIO(ROL_ID), -- Clave foránea hacia USUARIO
    FOREIGN KEY (PERM_ID) REFERENCES PERMISO(PERM_ID) -- Clave foránea hacia PERMISO
);

-- ====================================================================================
-- Procedimiento almacenado: sp_RolPermiso_Insertar
-- ====================================================================================
CREATE PROCEDURE sp_RolPermiso_Insertar
    @ROL_ID INT,
    @PERM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ROL_PERMISO (ROL_ID, PERM_ID)
    VALUES (@ROL_ID, @PERM_ID);
END
GO

-- ====================================================================================
-- Procedimiento almacenado: sp_RolPermiso_ObtenerPorUsuario
-- ====================================================================================
CREATE PROCEDURE sp_RolPermiso_ObtenerPorRol
    @ROL_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM ROL_PERMISO WHERE ROL_ID = @ROL_ID;
END
GO

-- ====================================================================================
-- Procedimiento almacenado: sp_RolPermiso_Eliminar
-- ====================================================================================
CREATE PROCEDURE sp_RolPermiso_Eliminar
    @ROL_ID INT,
    @PERM_ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM ROL_PERMISO WHERE ROL_ID = @ROL_ID AND PERM_ID = @PERM_ID;
END
GO

