use AdminV2

if exists (select 1
          from sysobjects
          where id = object_id('trg_AuditoriaInmueble_Delete')
          and type = 'TR')
   drop trigger trg_AuditoriaInmueble_Delete
go

if exists (select 1
          from sysobjects
          where id = object_id('trg_AuditoriaInmueble_Insert')
          and type = 'TR')
   drop trigger trg_AuditoriaInmueble_Insert
go

if exists (select 1
          from sysobjects
          where id = object_id('trg_AuditoriaInmueble_Update')
          and type = 'TR')
   drop trigger trg_AuditoriaInmueble_Update
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ApiLog') and o.name = 'FK_ApiLog_Usuario')
alter table ApiLog
   drop constraint FK_ApiLog_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ApiToken') and o.name = 'FK_ApiToken_Usuario')
alter table ApiToken
   drop constraint FK_ApiToken_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ArchivoTrabajo') and o.name = 'FK_ArchivoTrabajo_Trabajo')
alter table ArchivoTrabajo
   drop constraint FK_ArchivoTrabajo_Trabajo
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Auditoria_Inmueble') and o.name = 'FK_AuditoriaInmueble_Inmueble')
alter table Auditoria_Inmueble
   drop constraint FK_AuditoriaInmueble_Inmueble
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Auditoria_Usuario') and o.name = 'FK_AuditoriaUsuario_Usuario')
alter table Auditoria_Usuario
   drop constraint FK_AuditoriaUsuario_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Bitacora') and o.name = 'FK_Bitacora_Usuario')
alter table Bitacora
   drop constraint FK_Bitacora_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CalificacionInquilino') and o.name = 'FK_CalificacionInquilino_Contrato')
alter table CalificacionInquilino
   drop constraint FK_CalificacionInquilino_Contrato
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CalificacionInquilino') and o.name = 'FK_CalificacionInquilino_Usuario')
alter table CalificacionInquilino
   drop constraint FK_CalificacionInquilino_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CalificacionProveedor') and o.name = 'FK_CalificacionProveedor_Trabajo')
alter table CalificacionProveedor
   drop constraint FK_CalificacionProveedor_Trabajo
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CalificacionProveedor') and o.name = 'FK_CalificacionProveedor_Usuario')
alter table CalificacionProveedor
   drop constraint FK_CalificacionProveedor_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CaracteristicaInmueble') and o.name = 'FK_CaracteristicaInmueble_Caracteristica')
alter table CaracteristicaInmueble
   drop constraint FK_CaracteristicaInmueble_Caracteristica
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('CaracteristicaInmueble') and o.name = 'FK_CaracteristicaInmueble_Inmueble')
alter table CaracteristicaInmueble
   drop constraint FK_CaracteristicaInmueble_Inmueble
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ContratoRenta') and o.name = 'FK_ContratoRenta_Inmueble')
alter table ContratoRenta
   drop constraint FK_ContratoRenta_Inmueble
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ContratoRenta') and o.name = 'FK_ContratoRenta_Inquilino')
alter table ContratoRenta
   drop constraint FK_ContratoRenta_Inquilino
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ContratoRenta') and o.name = 'FK_ContratoRenta_Moneda')
alter table ContratoRenta
   drop constraint FK_ContratoRenta_Moneda
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('HistorialCambios') and o.name = 'FK_HistorialCambios_Usuario')
alter table HistorialCambios
   drop constraint FK_HistorialCambios_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Inmueble') and o.name = 'FK_Inmueble_Administrador')
alter table Inmueble
   drop constraint FK_Inmueble_Administrador
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Inmueble') and o.name = 'FK_Inmueble_Moneda')
alter table Inmueble
   drop constraint FK_Inmueble_Moneda
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('InmuebleCopropietario') and o.name = 'FK_InmuebleCopropietario_Empresa')
alter table InmuebleCopropietario
   drop constraint FK_InmuebleCopropietario_Empresa
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('InmuebleCopropietario') and o.name = 'FK_InmuebleCopropietario_Inmueble')
alter table InmuebleCopropietario
   drop constraint FK_InmuebleCopropietario_Inmueble
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('InmuebleCopropietario') and o.name = 'FK_InmuebleCopropietario_Persona')
alter table InmuebleCopropietario
   drop constraint FK_InmuebleCopropietario_Persona
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('PagoRenta') and o.name = 'FK_PagoRenta_Contrato')
alter table PagoRenta
   drop constraint FK_PagoRenta_Contrato
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('PagoRenta') and o.name = 'FK_PagoRenta_Moneda')
alter table PagoRenta
   drop constraint FK_PagoRenta_Moneda
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('PolizaSeguro') and o.name = 'FK_PolizaSeguro_Inmueble')
alter table PolizaSeguro
   drop constraint FK_PolizaSeguro_Inmueble
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Proveedor') and o.name = 'FK_Proveedor_Usuario')
alter table Proveedor
   drop constraint FK_Proveedor_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ServicioProveedor') and o.name = 'FK_ServicioProveedor_Proveedor')
alter table ServicioProveedor
   drop constraint FK_ServicioProveedor_Proveedor
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('ServicioProveedor') and o.name = 'FK_ServicioProveedor_TipoServicio')
alter table ServicioProveedor
   drop constraint FK_ServicioProveedor_TipoServicio
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TrabajoProveedor') and o.name = 'FK_TrabajoProveedor_Contrato')
alter table TrabajoProveedor
   drop constraint FK_TrabajoProveedor_Contrato
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TrabajoProveedor') and o.name = 'FK_TrabajoProveedor_Inmueble')
alter table TrabajoProveedor
   drop constraint FK_TrabajoProveedor_Inmueble
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('TrabajoProveedor') and o.name = 'FK_TrabajoProveedor_Proveedor')
alter table TrabajoProveedor
   drop constraint FK_TrabajoProveedor_Proveedor
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Usuario') and o.name = 'FK_Usuario_Empresa')
alter table Usuario
   drop constraint FK_Usuario_Empresa
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Usuario') and o.name = 'FK_Usuario_Moneda')
alter table Usuario
   drop constraint FK_Usuario_Moneda
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('Usuario') and o.name = 'FK_Usuario_Persona')
alter table Usuario
   drop constraint FK_Usuario_Persona
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('UsuarioRol') and o.name = 'FK_UsuarioRol_Rol')
alter table UsuarioRol
   drop constraint FK_UsuarioRol_Rol
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('UsuarioRol') and o.name = 'FK_UsuarioRol_Usuario')
alter table UsuarioRol
   drop constraint FK_UsuarioRol_Usuario
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('VerificacionInquilino') and o.name = 'FK_VerificacionInquilino_Contrato')
alter table VerificacionInquilino
   drop constraint FK_VerificacionInquilino_Contrato
go

if exists (select 1
   from sys.sysreferences r join sys.sysobjects o on (o.id = r.constid and o.type = 'F')
   where r.fkeyid = object_id('VerificacionInquilino') and o.name = 'FK_VerificacionInquilino_Usuario')
alter table VerificacionInquilino
   drop constraint FK_VerificacionInquilino_Usuario
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ApiLog')
            and   type = 'U')
   drop table ApiLog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ApiToken')
            and   type = 'U')
   drop table ApiToken
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ArchivoTrabajo')
            and   type = 'U')
   drop table ArchivoTrabajo
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Auditoria_Inmueble')
            and   type = 'U')
   drop table Auditoria_Inmueble
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Auditoria_Usuario')
            and   type = 'U')
   drop table Auditoria_Usuario
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Bitacora')
            and   type = 'U')
   drop table Bitacora
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CalificacionInquilino')
            and   type = 'U')
   drop table CalificacionInquilino
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CalificacionProveedor')
            and   type = 'U')
   drop table CalificacionProveedor
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Caracteristica')
            and   type = 'U')
   drop table Caracteristica
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CaracteristicaInmueble')
            and   type = 'U')
   drop table CaracteristicaInmueble
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ContratoRenta')
            and   type = 'U')
   drop table ContratoRenta
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Empresa')
            and   type = 'U')
   drop table Empresa
go

if exists (select 1
            from  sysobjects
           where  id = object_id('HistorialCambios')
            and   type = 'U')
   drop table HistorialCambios
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Inmueble')
            and   type = 'U')
   drop table Inmueble
go

if exists (select 1
            from  sysobjects
           where  id = object_id('InmuebleCopropietario')
            and   type = 'U')
   drop table InmuebleCopropietario
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Moneda')
            and   type = 'U')
   drop table Moneda
go

if exists (select 1
            from  sysobjects
           where  id = object_id('PagoRenta')
            and   type = 'U')
   drop table PagoRenta
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Persona')
            and   type = 'U')
   drop table Persona
go

if exists (select 1
            from  sysobjects
           where  id = object_id('PolizaSeguro')
            and   type = 'U')
   drop table PolizaSeguro
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Proveedor')
            and   type = 'U')
   drop table Proveedor
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Rol')
            and   type = 'U')
   drop table Rol
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ServicioProveedor')
            and   type = 'U')
   drop table ServicioProveedor
go

if exists (select 1
            from  sysobjects
           where  id = object_id('TipoServicio')
            and   type = 'U')
   drop table TipoServicio
go

if exists (select 1
            from  sysobjects
           where  id = object_id('TrabajoProveedor')
            and   type = 'U')
   drop table TrabajoProveedor
go

if exists (select 1
            from  sysobjects
           where  id = object_id('Usuario')
            and   type = 'U')
   drop table Usuario
go

if exists (select 1
            from  sysobjects
           where  id = object_id('UsuarioRol')
            and   type = 'U')
   drop table UsuarioRol
go

if exists (select 1
            from  sysobjects
           where  id = object_id('VerificacionInquilino')
            and   type = 'U')
   drop table VerificacionInquilino
go

/*==============================================================*/
/* Table: ApiLog                                                */
/*==============================================================*/
create table ApiLog (
   ApiLogID             INT                  identity(1,1),
   UsuarioID            INT                  null,
   Servicio             VARCHAR(50)          null,
   Metodo               VARCHAR(50)          null,
   Endpoint             NVARCHAR(300)        null,
   RequestBody          NVARCHAR(MAX)        null,
   ResponseBody         NVARCHAR(MAX)        null,
   Fecha                DATETIME             null default getdate(),
   constraint PK_ApiLog primary key (ApiLogID)
)
go

/*==============================================================*/
/* Table: ApiToken                                              */
/*==============================================================*/
create table ApiToken (
   ApiTokenID           INT                  identity(1,1),
   UsuarioID            INT                  not null,
   NombreServicio       VARCHAR(50)          null,
   Token                NVARCHAR(500)        null,
   FechaExpiracion      DATETIME             null,
   Activo               BIT                  null default 1,
   FechaCreacion        DATETIME             null default getdate(),
   constraint PK_ApiToken primary key (ApiTokenID)
)
go

/*==============================================================*/
/* Table: ArchivoTrabajo                                        */
/*==============================================================*/
create table ArchivoTrabajo (
   ArchivoTrabajoID     INT                  identity(1,1),
   TrabajoID            INT                  not null,
   Tipo                 VARCHAR(20)          null,
   Url                  NVARCHAR(400)        null,
   Descripcion          NVARCHAR(500)        null,
   constraint PK_ArchivoTrabajo primary key (ArchivoTrabajoID)
)
go

/*==============================================================*/
/* Table: Auditoria_Inmueble                                    */
/*==============================================================*/
create table Auditoria_Inmueble (
   AuditoriaInmuebleID  INT                  identity(1,1),
   InmuebleID           INT                  null,
   Fecha                DATETIME             null default getdate(),
   Accion               VARCHAR(50)          null,
   DatosAnteriores      NVARCHAR(MAX)        null,
   DatosNuevos          NVARCHAR(MAX)        null,
   UsuarioEditorID      INT                  null,
   constraint PK_AuditoriaInmueble primary key (AuditoriaInmuebleID)
)
go

/*==============================================================*/
/* Table: Auditoria_Usuario                                     */
/*==============================================================*/
create table Auditoria_Usuario (
   AuditoriaUsuarioID   INT                  identity(1,1),
   UsuarioID            INT                  null,
   Fecha                DATETIME             null default getdate(),
   Accion               VARCHAR(50)          null,
   DatosAnteriores      NVARCHAR(MAX)        null,
   DatosNuevos          NVARCHAR(MAX)        null,
   UsuarioEditorID      INT                  null,
   constraint PK_AuditoriaUsuario primary key (AuditoriaUsuarioID)
)
go

/*==============================================================*/
/* Table: Bitacora                                              */
/*==============================================================*/
create table Bitacora (
   BitacoraID           INT                  identity(1,1),
   UsuarioID            INT                  not null,
   FechaHora            DATETIME             null default getdate(),
   Accion               VARCHAR(100)         null,
   Entidad              VARCHAR(50)          null,
   EntidadID            INT                  null,
   Descripcion          NVARCHAR(500)        null,
   constraint PK_Bitacora primary key (BitacoraID)
)
go

/*==============================================================*/
/* Table: CalificacionInquilino                                 */
/*==============================================================*/
create table CalificacionInquilino (
   CalificacionInquilinoID INT                  identity(1,1),
   ContratoID           INT                  not null,
   CalificadorID        INT                  not null,
   TipoCalificador      VARCHAR(20)          null,
   Calificacion         INT                  null,
   Comentario           NVARCHAR(500)        null,
   Fecha                DATETIME             null default getdate(),
   constraint PK_CalificacionInquilino primary key (CalificacionInquilinoID)
)
go

/*==============================================================*/
/* Table: CalificacionProveedor                                 */
/*==============================================================*/
create table CalificacionProveedor (
   CalificacionProveedorID INT                  identity(1,1),
   TrabajoID            INT                  not null,
   CalificadorID        INT                  not null,
   Calificacion         INT                  null,
   Comentario           NVARCHAR(500)        null,
   Fecha                DATETIME             null default getdate(),
   constraint PK_CalificacionProveedor primary key (CalificacionProveedorID)
)
go

/*==============================================================*/
/* Table: Caracteristica                                        */
/*==============================================================*/
create table Caracteristica (
   CaracteristicaID     INT                  identity(1,1),
   Nombre               VARCHAR(50)          null,
   Tipo                 VARCHAR(20)          null 
      constraint CKC_TIPO_CARACTER check (Tipo is null or (Tipo in ('booleano','numerico','texto'))),
   Descripcion          NVARCHAR(300)        null,
   constraint PK_Caracteristica primary key (CaracteristicaID)
)
go

/*==============================================================*/
/* Table: CaracteristicaInmueble                                */
/*==============================================================*/
create table CaracteristicaInmueble (
   CaracteristicaInmuebleID INT                  identity(1,1),
   InmuebleID           INT                  not null,
   CaracteristicaID     INT                  not null,
   Valor                VARCHAR(50)          null,
   constraint PK_CaracteristicaInmueble primary key (CaracteristicaInmuebleID)
)
go

/*==============================================================*/
/* Table: ContratoRenta                                         */
/*==============================================================*/
create table ContratoRenta (
   ContratoRentaID      INT                  identity(1,1),
   InmuebleID           INT                  not null,
   InquilinoID          INT                  not null,
   FechaInicio          DATE                 null,
   FechaFin             DATE                 null,
   MontoRenta           DECIMAL(15,2)        null,
   Condiciones          NVARCHAR(1000)       null,
   Deposito             DECIMAL(15,2)        null,
   Estado               VARCHAR(20)          null,
   DocFirmadoUrl        NVARCHAR(400)        null,
   PolizaRentaSeguraID  INT                  null,
   MonedaID             INT                  not null default 1,
   FechaCreacion        DATETIME             null default getdate(),
   FechaModificacion    DATETIME             null default getdate(),
   UsuarioCreadorID     INT                  null,
   UsuarioModificadorID INT                  null,
   constraint PK_ContratoRenta primary key (ContratoRentaID)
)
go

/*==============================================================*/
/* Table: Empresa                                               */
/*==============================================================*/
create table Empresa (
   EmpresaID            INT                  identity(1,1),
   RazonSocial          VARCHAR(100)         null,
   RFC                  VARCHAR(20)          null,
   Email                VARCHAR(100)         null,
   Telefono             VARCHAR(30)          null,
   Direccion            NVARCHAR(500)        null,
   constraint PK_Empresa primary key (EmpresaID)
)
go

/*==============================================================*/
/* Table: HistorialCambios                                      */
/*==============================================================*/
create table HistorialCambios (
   HistorialCambiosID   INT                  identity(1,1),
   Entidad              VARCHAR(100)         null,
   EntidadID            INT                  null,
   UsuarioID            INT                  null,
   Fecha                DATETIME             null default getdate(),
   Cambios              NVARCHAR(MAX)        null,
   constraint PK_HistorialCambios primary key (HistorialCambiosID)
)
go

/*==============================================================*/
/* Table: Inmueble                                              */
/*==============================================================*/
create table Inmueble (
   InmuebleID           INT                  identity(1,1),
   Nombre               VARCHAR(100)         null,
   Direccion            NVARCHAR(500)        null,
   Pais                 VARCHAR(50)          null,
   Estado               VARCHAR(50)          null,
   Ciudad               VARCHAR(50)          null,
   CP                   VARCHAR(10)          null,
   Latitud              DECIMAL(10,6)        null,
   Longitud             DECIMAL(10,6)        null,
   Valor                DECIMAL(15,2)        null,
   ConstruccionM2       DECIMAL(10,2)        null,
   RentaMensual         DECIMAL(15,2)        null,
   AdministradorID      INT                  null,
   Descripcion          NVARCHAR(1000)       null,
   MonedaID             INT                  not null default 1,
   Activo               BIT                  null default 1,
   FechaCreacion        DATETIME             null default getdate(),
   FechaModificacion    DATETIME             null default getdate(),
   UsuarioCreadorID     INT                  null,
   UsuarioModificadorID INT                  null,
   constraint PK_Inmueble primary key (InmuebleID)
)
go

/*==============================================================*/
/* Table: InmuebleCopropietario                                 */
/*==============================================================*/
create table InmuebleCopropietario (
   InmuebleCopropietarioID INT                  identity(1,1),
   InmuebleID           INT                  not null,
   PersonaID            INT                  null,
   EmpresaID            INT                  null,
   Porcentaje           DECIMAL(5,2)         not null,
   FechaCreacion        DATETIME             null default getdate(),
   UsuarioCreadorID     INT                  null,
   constraint PK_InmuebleCopropietario primary key (InmuebleCopropietarioID)
)
go

/*==============================================================*/
/* Table: Moneda                                                */
/*==============================================================*/
create table Moneda (
   MonedaID             INT                  identity(1,1),
   Codigo               VARCHAR(10)          not null,
   Nombre               VARCHAR(50)          not null,
   constraint PK_Moneda primary key (MonedaID)
)
go

/*==============================================================*/
/* Table: PagoRenta                                             */
/*==============================================================*/
create table PagoRenta (
   PagoRentaID          INT                  identity(1,1),
   ContratoID           INT                  not null,
   FechaPago            DATE                 null,
   Monto                DECIMAL(15,2)        null,
   MetodoPago           VARCHAR(50)          null,
   EsParcial            BIT                  null,
   Atraso               BIT                  null,
   Penalizacion         DECIMAL(15,2)        null,
   ComprobanteUrl       NVARCHAR(400)        null,
   MonedaID             INT                  not null default 1,
   FechaCreacion        DATETIME             null default getdate(),
   FechaModificacion    DATETIME             null default getdate(),
   UsuarioCreadorID     INT                  null,
   UsuarioModificadorID INT                  null,
   constraint PK_PagoRenta primary key (PagoRentaID)
)
go

/*==============================================================*/
/* Table: Persona                                               */
/*==============================================================*/
create table Persona (
   PersonaID            INT                  identity(1,1),
   Nombre               VARCHAR(100)         null,
   RFC                  VARCHAR(20)          null,
   Email                VARCHAR(100)         null,
   Telefono             VARCHAR(30)          null,
   Direccion            NVARCHAR(500)        null,
   constraint PK_Persona primary key (PersonaID)
)
go

/*==============================================================*/
/* Table: PolizaSeguro                                          */
/*==============================================================*/
create table PolizaSeguro (
   PolizaSeguroID       INT                  identity(1,1),
   InmuebleID           INT                  not null,
   Tipo                 VARCHAR(20)          null,
   NumPoliza            VARCHAR(50)          null,
   Aseguradora          VARCHAR(100)         null,
   VigenciaInicio       DATE                 null,
   VigenciaFin          DATE                 null,
   MontoAsegurado       DECIMAL(15,2)        null,
   ArchivoUrl           NVARCHAR(400)        null,
   Estado               VARCHAR(20)          null,
   FechaCreacion        DATETIME             null default getdate(),
   UsuarioCreadorID     INT                  null,
   constraint PK_PolizaSeguro primary key (PolizaSeguroID)
)
go

/*==============================================================*/
/* Table: Proveedor                                             */
/*==============================================================*/
create table Proveedor (
   ProveedorID          INT                  identity(1,1),
   Nombre               VARCHAR(100)         null,
   RFC                  VARCHAR(20)          null,
   Tipo                 VARCHAR(20)          null,
   Email                VARCHAR(100)         null,
   Telefono             VARCHAR(30)          null,
   Direccion            NVARCHAR(500)        null,
   UsuarioID            INT                  null,
   Activo               BIT                  null default 1,
   FechaCreacion        DATETIME             null default getdate(),
   FechaModificacion    DATETIME             null default getdate(),
   UsuarioCreadorID     INT                  null,
   UsuarioModificadorID INT                  null,
   constraint PK_Proveedor primary key (ProveedorID)
)
go

/*==============================================================*/
/* Table: Rol                                                   */
/*==============================================================*/
create table Rol (
   RolID                INT                  identity(1,1),
   Nombre               VARCHAR(50)          not null,
   constraint PK_Rol primary key (RolID)
)
go

/*==============================================================*/
/* Table: ServicioProveedor                                     */
/*==============================================================*/
create table ServicioProveedor (
   ServicioProveedorID  INT                  identity(1,1),
   ProveedorID          INT                  not null,
   TipoServicioID       INT                  not null,
   constraint PK_ServicioProveedor primary key (ServicioProveedorID)
)
go

/*==============================================================*/
/* Table: TipoServicio                                          */
/*==============================================================*/
create table TipoServicio (
   TipoServicioID       INT                  identity(1,1),
   Nombre               VARCHAR(50)          null,
   Descripcion          NVARCHAR(300)        null,
   constraint PK_TipoServicio primary key (TipoServicioID)
)
go

/*==============================================================*/
/* Table: TrabajoProveedor                                      */
/*==============================================================*/
create table TrabajoProveedor (
   TrabajoProveedorID   INT                  identity(1,1),
   InmuebleID           INT                  not null,
   ProveedorID          INT                  not null,
   Fecha                DATETIME             null default getdate(),
   Descripcion          NVARCHAR(1000)       null,
   Estado               VARCHAR(20)          null,
   Costo                DECIMAL(15,2)        null,
   ContratoID           INT                  null,
   FacturaUrl           NVARCHAR(400)        null,
   FechaCreacion        DATETIME             null default getdate(),
   UsuarioCreadorID     INT                  null,
   constraint PK_TrabajoProveedor primary key (TrabajoProveedorID)
)
go

/*==============================================================*/
/* Table: Usuario                                               */
/*==============================================================*/
create table Usuario (
   UsuarioID            INT                  identity(1,1),
   Nombre               VARCHAR(100)         null,
   Email                VARCHAR(100)         not null,
   Password             VARCHAR(200)         not null,
   Pais                 VARCHAR(50)          null,
   Telefono             VARCHAR(30)          null,
   PersonaID            INT                  null,
   EmpresaID            INT                  null,
   MonedaID             INT                  not null default 1,
   Activo               BIT                  null default 1,
   FechaCreacion        DATETIME             null default getdate(),
   FechaModificacion    DATETIME             null default getdate(),
   UsuarioCreadorID     INT                  null,
   UsuarioModificadorID INT                  null,
   constraint PK_Usuario primary key (UsuarioID),
   constraint UQ_Usuario_Email unique (Email)
)
go

/*==============================================================*/
/* Table: UsuarioRol                                            */
/*==============================================================*/
create table UsuarioRol (
   UsuarioRolID         INT                  identity(1,1),
   UsuarioID            INT                  not null,
   RolID                INT                  not null,
   constraint PK_UsuarioRol primary key (UsuarioRolID)
)
go

/*==============================================================*/
/* Table: VerificacionInquilino                                 */
/*==============================================================*/
create table VerificacionInquilino (
   VerificacionInquilinoID INT                  identity(1,1),
   ContratoID           INT                  not null,
   TipoVerificacion     VARCHAR(50)          null,
   Resultado            VARCHAR(50)          null,
   Fecha                DATETIME             null default getdate(),
   UsuarioID            INT                  not null,
   ArchivoUrl           NVARCHAR(400)        null,
   constraint PK_VerificacionInquilino primary key (VerificacionInquilinoID)
)
go

alter table ApiLog
   add constraint FK_ApiLog_Usuario foreign key (UsuarioID)
      references Usuario (UsuarioID)
go

alter table ApiToken
   add constraint FK_ApiToken_Usuario foreign key (UsuarioID)
      references Usuario (UsuarioID)
go

alter table ArchivoTrabajo
   add constraint FK_ArchivoTrabajo_Trabajo foreign key (TrabajoID)
      references TrabajoProveedor (TrabajoProveedorID)
go

alter table Auditoria_Inmueble
   add constraint FK_AuditoriaInmueble_Inmueble foreign key (InmuebleID)
      references Inmueble (InmuebleID)
go

alter table Auditoria_Usuario
   add constraint FK_AuditoriaUsuario_Usuario foreign key (UsuarioID)
      references Usuario (UsuarioID)
go

alter table Bitacora
   add constraint FK_Bitacora_Usuario foreign key (UsuarioID)
      references Usuario (UsuarioID)
go

alter table CalificacionInquilino
   add constraint FK_CalificacionInquilino_Contrato foreign key (ContratoID)
      references ContratoRenta (ContratoRentaID)
go

alter table CalificacionInquilino
   add constraint FK_CalificacionInquilino_Usuario foreign key (CalificadorID)
      references Usuario (UsuarioID)
go

alter table CalificacionProveedor
   add constraint FK_CalificacionProveedor_Trabajo foreign key (TrabajoID)
      references TrabajoProveedor (TrabajoProveedorID)
go

alter table CalificacionProveedor
   add constraint FK_CalificacionProveedor_Usuario foreign key (CalificadorID)
      references Usuario (UsuarioID)
go

alter table CaracteristicaInmueble
   add constraint FK_CaracteristicaInmueble_Caracteristica foreign key (CaracteristicaID)
      references Caracteristica (CaracteristicaID)
go

alter table CaracteristicaInmueble
   add constraint FK_CaracteristicaInmueble_Inmueble foreign key (InmuebleID)
      references Inmueble (InmuebleID)
go

alter table ContratoRenta
   add constraint FK_ContratoRenta_Inmueble foreign key (InmuebleID)
      references Inmueble (InmuebleID)
go

alter table ContratoRenta
   add constraint FK_ContratoRenta_Inquilino foreign key (InquilinoID)
      references Usuario (UsuarioID)
go

alter table ContratoRenta
   add constraint FK_ContratoRenta_Moneda foreign key (MonedaID)
      references Moneda (MonedaID)
go

alter table HistorialCambios
   add constraint FK_HistorialCambios_Usuario foreign key (UsuarioID)
      references Usuario (UsuarioID)
go

alter table Inmueble
   add constraint FK_Inmueble_Administrador foreign key (AdministradorID)
      references Usuario (UsuarioID)
go

alter table Inmueble
   add constraint FK_Inmueble_Moneda foreign key (MonedaID)
      references Moneda (MonedaID)
go

alter table InmuebleCopropietario
   add constraint FK_InmuebleCopropietario_Empresa foreign key (EmpresaID)
      references Empresa (EmpresaID)
go

alter table InmuebleCopropietario
   add constraint FK_InmuebleCopropietario_Inmueble foreign key (InmuebleID)
      references Inmueble (InmuebleID)
go

alter table InmuebleCopropietario
   add constraint FK_InmuebleCopropietario_Persona foreign key (PersonaID)
      references Persona (PersonaID)
go

alter table PagoRenta
   add constraint FK_PagoRenta_Contrato foreign key (ContratoID)
      references ContratoRenta (ContratoRentaID)
go

alter table PagoRenta
   add constraint FK_PagoRenta_Moneda foreign key (MonedaID)
      references Moneda (MonedaID)
go

alter table PolizaSeguro
   add constraint FK_PolizaSeguro_Inmueble foreign key (InmuebleID)
      references Inmueble (InmuebleID)
go

alter table Proveedor
   add constraint FK_Proveedor_Usuario foreign key (UsuarioID)
      references Usuario (UsuarioID)
go

alter table ServicioProveedor
   add constraint FK_ServicioProveedor_Proveedor foreign key (ProveedorID)
      references Proveedor (ProveedorID)
go

alter table ServicioProveedor
   add constraint FK_ServicioProveedor_TipoServicio foreign key (TipoServicioID)
      references TipoServicio (TipoServicioID)
go

alter table TrabajoProveedor
   add constraint FK_TrabajoProveedor_Contrato foreign key (ContratoID)
      references ContratoRenta (ContratoRentaID)
go

alter table TrabajoProveedor
   add constraint FK_TrabajoProveedor_Inmueble foreign key (InmuebleID)
      references Inmueble (InmuebleID)
go

alter table TrabajoProveedor
   add constraint FK_TrabajoProveedor_Proveedor foreign key (ProveedorID)
      references Proveedor (ProveedorID)
go

alter table Usuario
   add constraint FK_Usuario_Empresa foreign key (EmpresaID)
      references Empresa (EmpresaID)
go

alter table Usuario
   add constraint FK_Usuario_Moneda foreign key (MonedaID)
      references Moneda (MonedaID)
go

alter table Usuario
   add constraint FK_Usuario_Persona foreign key (PersonaID)
      references Persona (PersonaID)
go

alter table UsuarioRol
   add constraint FK_UsuarioRol_Rol foreign key (RolID)
      references Rol (RolID)
go

alter table UsuarioRol
   add constraint FK_UsuarioRol_Usuario foreign key (UsuarioID)
      references Usuario (UsuarioID)
go

alter table VerificacionInquilino
   add constraint FK_VerificacionInquilino_Contrato foreign key (ContratoID)
      references ContratoRenta (ContratoRentaID)
go

alter table VerificacionInquilino
   add constraint FK_VerificacionInquilino_Usuario foreign key (UsuarioID)
      references Usuario (UsuarioID)
go


create trigger trg_AuditoriaInmueble_Delete on Inmueble after delete
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Auditoria_Inmueble (InmuebleID, Fecha, Accion, DatosAnteriores, DatosNuevos, UsuarioEditorID)
    SELECT 
        d.InmuebleID,
        GETDATE(),
        'DELETE',
        (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        NULL,
        d.UsuarioModificadorID -- Usa UsuarioModificadorID o crea un campo especial para este caso
    FROM deleted d;

    INSERT INTO HistorialCambios (Entidad, EntidadID, UsuarioID, Fecha, Cambios)
    SELECT
        'Inmueble',
        d.InmuebleID,
        d.UsuarioModificadorID,
        GETDATE(),
        (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM deleted d;
END;
go


create trigger trg_AuditoriaInmueble_Insert on Inmueble after insert
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Auditoria_Inmueble (InmuebleID, Fecha, Accion, DatosAnteriores, DatosNuevos, UsuarioEditorID)
    SELECT 
        i.InmuebleID,
        GETDATE(),
        'INSERT',
        NULL,
        (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        i.UsuarioCreadorID
    FROM inserted i;

    INSERT INTO HistorialCambios (Entidad, EntidadID, UsuarioID, Fecha, Cambios)
    SELECT
        'Inmueble',
        i.InmuebleID,
        i.UsuarioCreadorID,
        GETDATE(),
        (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted i;
END;
go


CREATE OR ALTER TRIGGER trg_AuditoriaInmueble_Update
ON Inmueble
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO HistorialCambios (Entidad, EntidadID, UsuarioID, Fecha, Cambios)
    SELECT
        'Inmueble',
        i.InmuebleID,
        i.UsuarioModificadorID,
        GETDATE(),
        (
            SELECT
                d.InmuebleID           AS OldInmuebleID,
                i.InmuebleID           AS NewInmuebleID,

                d.Nombre               AS OldNombre,
                i.Nombre               AS NewNombre,

                d.Direccion            AS OldDireccion,
                i.Direccion            AS NewDireccion,

                d.Pais                 AS OldPais,
                i.Pais                 AS NewPais,

                d.Estado               AS OldEstado,
                i.Estado               AS NewEstado,

                d.Ciudad               AS OldCiudad,
                i.Ciudad               AS NewCiudad,

                d.CP                   AS OldCP,
                i.CP                   AS NewCP,

                d.Latitud              AS OldLatitud,
                i.Latitud              AS NewLatitud,

                d.Longitud             AS OldLongitud,
                i.Longitud             AS NewLongitud,

                d.Valor                AS OldValor,
                i.Valor                AS NewValor,

                d.ConstruccionM2       AS OldConstruccionM2,
                i.ConstruccionM2       AS NewConstruccionM2,

                d.RentaMensual         AS OldRentaMensual,
                i.RentaMensual         AS NewRentaMensual,

                d.AdministradorID      AS OldAdministradorID,
                i.AdministradorID      AS NewAdministradorID,

                d.Descripcion          AS OldDescripcion,
                i.Descripcion          AS NewDescripcion,

                d.MonedaID             AS OldMonedaID,
                i.MonedaID             AS NewMonedaID,

                d.Activo               AS OldActivo,
                i.Activo               AS NewActivo,

                d.FechaCreacion        AS OldFechaCreacion,
                i.FechaCreacion        AS NewFechaCreacion,

                d.FechaModificacion    AS OldFechaModificacion,
                i.FechaModificacion    AS NewFechaModificacion,

                d.UsuarioCreadorID     AS OldUsuarioCreadorID,
                i.UsuarioCreadorID     AS NewUsuarioCreadorID,

                d.UsuarioModificadorID AS OldUsuarioModificadorID,
                i.UsuarioModificadorID AS NewUsuarioModificadorID

            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        )
    FROM inserted i
    INNER JOIN deleted d ON i.InmuebleID = d.InmuebleID;
END;
