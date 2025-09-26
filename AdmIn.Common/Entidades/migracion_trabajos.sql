-- Script de migración para nuevas entidades y modificaciones

-- 1. Tabla DetalleTrabajo
CREATE TABLE DetalleTrabajo (
    DetalleTrabajoId INT IDENTITY PRIMARY KEY,
    TrabajoProveedorId INT NOT NULL,
    Fecha DATETIME NOT NULL,
    Descripcion NVARCHAR(1000) NOT NULL,
    Costo DECIMAL(18,2) NOT NULL,
    Estado NVARCHAR(50) NOT NULL,
    UsuarioId INT NOT NULL,
    CONSTRAINT FK_DetalleTrabajo_TrabajoProveedor FOREIGN KEY (TrabajoProveedorId) REFERENCES TrabajoProveedor(TrabajoProveedorId),
    CONSTRAINT FK_DetalleTrabajo_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId)
);

-- 2. Tabla HistorialTrabajo
CREATE TABLE HistorialTrabajo (
    HistorialTrabajoId INT IDENTITY PRIMARY KEY,
    TrabajoProveedorId INT NOT NULL,
    Fecha DATETIME NOT NULL,
    Estado NVARCHAR(50) NOT NULL,
    UsuarioId INT NOT NULL,
    Comentario NVARCHAR(1000) NULL,
    CONSTRAINT FK_HistorialTrabajo_TrabajoProveedor FOREIGN KEY (TrabajoProveedorId) REFERENCES TrabajoProveedor(TrabajoProveedorId),
    CONSTRAINT FK_HistorialTrabajo_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId)
);

-- 3. Tabla HistorialDetalleTrabajo
CREATE TABLE HistorialDetalleTrabajo (
    HistorialDetalleTrabajoId INT IDENTITY PRIMARY KEY,
    DetalleTrabajoId INT NOT NULL,
    Fecha DATETIME NOT NULL,
    Estado NVARCHAR(50) NOT NULL,
    UsuarioId INT NOT NULL,
    Comentario NVARCHAR(1000) NULL,
    CONSTRAINT FK_HistorialDetalleTrabajo_DetalleTrabajo FOREIGN KEY (DetalleTrabajoId) REFERENCES DetalleTrabajo(DetalleTrabajoId),
    CONSTRAINT FK_HistorialDetalleTrabajo_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId)
);

-- 4. Tabla CalificacionProveedor
CREATE TABLE CalificacionProveedor (
    CalificacionProveedorId INT IDENTITY PRIMARY KEY,
    TrabajoProveedorId INT NOT NULL,
    ProveedorId INT NOT NULL,
    UsuarioId INT NOT NULL,
    Valor INT NOT NULL,
    Comentario NVARCHAR(1000) NULL,
    Fecha DATETIME NOT NULL,
    CONSTRAINT FK_CalificacionProveedor_TrabajoProveedor FOREIGN KEY (TrabajoProveedorId) REFERENCES TrabajoProveedor(TrabajoProveedorId),
    CONSTRAINT FK_CalificacionProveedor_Proveedor FOREIGN KEY (ProveedorId) REFERENCES Proveedor(ProveedorId),
    CONSTRAINT FK_CalificacionProveedor_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId)
);

-- 5. Tabla TrabajoProveedor: agregar columnas
ALTER TABLE TrabajoProveedor ADD 
    FechaInicio DATETIME NULL,
    CostoAproximado DECIMAL(18,2) NULL;

-- 6. Tabla TrabajoProveedor_Imagen (relación muchos a muchos)
CREATE TABLE TrabajoProveedor_Imagen (
    TrabajoProveedorId INT NOT NULL,
    ImagenId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (TrabajoProveedorId, ImagenId),
    CONSTRAINT FK_TrabajoProveedorImagen_TrabajoProveedor FOREIGN KEY (TrabajoProveedorId) REFERENCES TrabajoProveedor(TrabajoProveedorId),
    CONSTRAINT FK_TrabajoProveedorImagen_Imagen FOREIGN KEY (ImagenId) REFERENCES Imagen(Id)
);

-- 7. Tabla DetalleTrabajo_Imagen (relación muchos a muchos)
CREATE TABLE DetalleTrabajo_Imagen (
    DetalleTrabajoId INT NOT NULL,
    ImagenId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (DetalleTrabajoId, ImagenId),
    CONSTRAINT FK_DetalleTrabajoImagen_DetalleTrabajo FOREIGN KEY (DetalleTrabajoId) REFERENCES DetalleTrabajo(DetalleTrabajoId),
    CONSTRAINT FK_DetalleTrabajoImagen_Imagen FOREIGN KEY (ImagenId) REFERENCES Imagen(Id)
);
