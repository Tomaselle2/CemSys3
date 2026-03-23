create database cemsys;
go
use cemsys;
go
-- Script para crear todas las tablas del sistema de cementerio
-- Orden basado en dependencias de claves foráneas

-- ==========================================
-- CONFIGURACIÓN PREVIA FILESTREAM
-- ==========================================

-- NOTA: Antes de ejecutar este script
-- 1. Habilitar FILESTREAM en la instancia de SQL Server
-- 2. Cambiar 'NombreTuBaseDeDatos' por el nombre real de tu base de datos
-- 3. Cambiar 'C:\FileStreamData' por la ruta donde quieres almacenar los archivos

-- Agregar filegroup FILESTREAM a la base de datos
ALTER DATABASE [cemsys] 
ADD FILEGROUP [CementerioFileStreamGroupArchive] CONTAINS FILESTREAM;

go

-- Agregar archivo físico para FILESTREAM
ALTER DATABASE [cemsys] 
ADD FILE (
    NAME = 'CementerioFileStreamFile',
    FILENAME = 'C:\CemsysArchive3' -- RUTA
) TO FILEGROUP [CementerioFileStreamGroupArchive];

PRINT 'Configuración FILESTREAM completada.';

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'dbo') 
BEGIN 
    EXEC('CREATE SCHEMA [dbo]'); 
END;

-- Tablas sin dependencias
CREATE TABLE [dbo].[CantidadCuotas] (
    [id] int NOT NULL IDENTITY(1,1),
    [cuota] int NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[TemasTarifaria] (
    [id] int NOT NULL IDENTITY(1,1),
    [nombre] nvarchar(50) NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[RolesUsuarios] (
    [id] int NOT NULL IDENTITY(1,1),
    [rol] nvarchar(30) NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[TipoNumeracionParcelas] (
    [id] int NOT NULL IDENTITY(1,1),
    [tipoNumeracion] nvarchar(30) NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[TipoNichos] (
    [id] int NOT NULL IDENTITY(1,1),
    [tipo] nvarchar(20) NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[AnioConcesion] (
    [id] int NOT NULL IDENTITY(1,1),
    [anios] int NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[CategoriasPersonas] (
    [id] int NOT NULL IDENTITY(1,1),
    [categoria] nvarchar(30) NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[TipoNota] (
    [id] int NOT NULL IDENTITY(1,1),
    [Descripcion] nvarchar(30) NOT NULL,
    [visibilidad] bit NOT NULL,
    PRIMARY KEY ([id])
);


CREATE TABLE [dbo].[EstadosDifunto] (
    [id] int NOT NULL IDENTITY(1,1),
    [estado] nvarchar(30) NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[TipoTramite] (
    [id] int NOT NULL IDENTITY(1,1),
    [tipo] nvarchar(50) NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[TipoParcela] (
    [id] int NOT NULL IDENTITY(1,1),
    [tipo] nvarchar(30) NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[TipoPanteon] (
    [id] int NOT NULL IDENTITY(1,1),
    [tipo] nvarchar(20) NOT NULL,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[EmpresasFunebres] (
    [id] int NOT NULL IDENTITY(1,1),
    [nombre] nvarchar(50) NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    PRIMARY KEY ([id])
);

CREATE TABLE [dbo].[Cementerios] (
    [id] int NOT NULL IDENTITY(1,1),
    [nombre] nvarchar(50) NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    PRIMARY KEY ([id])
);

-- Tablas con dependencias de primer nivel
CREATE TABLE [dbo].[Usuarios] (
    [id] int NOT NULL IDENTITY(1,1),
    [nombre] nvarchar(50) NOT NULL,
    [apellido] nvarchar(50) NOT NULL,
    [correo] nvarchar(50) NOT NULL,
    [usuario] nvarchar(50) NOT NULL,
    [clave] nvarchar(max) NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    [rolId] int NOT NULL,
    PRIMARY KEY ([id]),
    CONSTRAINT [Usuarios_rol_fk] FOREIGN KEY([rolId]) REFERENCES [dbo].[RolesUsuarios]([id])
);

CREATE TABLE [dbo].[Personas] (
    [id] int NOT NULL IDENTITY(1,1),
    [nombre] nvarchar(50),
    [apellido] nvarchar(50),
    [dni] nvarchar(15),
    [visibilidad] bit NOT NULL DEFAULT 1,
    [fechaNacimiento] date,
    [fechaDefuncion] date,
    [informacionAdicional] nvarchar(max),
    [sexo] nvarchar(15),
    [correo] nvarchar(50),
    [celular] nvarchar(50),
    [domicilio] nvarchar(100),
    [nroActa] int,
    [nroFolio] int,
    [nroTomo] int,
    [nroSerie] nvarchar(5),
    [nroAge] int,
    [estadoDifuntoId] int,
    [categoriaPersonaId] int,
    PRIMARY KEY ([id]),
    CONSTRAINT [Personas_categoriaPersonaId_fk] FOREIGN KEY([categoriaPersonaId]) REFERENCES [dbo].[CategoriasPersonas]([id]),
    CONSTRAINT [Personas_estadoDifuntoId_fk] FOREIGN KEY([estadoDifuntoId]) REFERENCES [dbo].[EstadosDifunto]([id])
);


CREATE TABLE [dbo].[EstadosTramites] (
    [id] int NOT NULL IDENTITY(1,1),
    [estado] nvarchar(50) NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    [tipoTramiteId] int NOT NULL,
    PRIMARY KEY ([id]),
    CONSTRAINT [EstadosTramites_tipoTramiteId_fk] FOREIGN KEY([tipoTramiteId]) REFERENCES [dbo].[TipoTramite]([id])
);

CREATE TABLE [dbo].[Tramites] (
    [id] int NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    [fechaCreacion] datetime NOT NULL,
    [tipoTramiteId] int NOT NULL,
    [usuarioId] int NOT NULL,
    [estadoActualId] int NOT NULL,
    PRIMARY KEY ([id]),
    CONSTRAINT [Tramites_estadoActualId_fk] FOREIGN KEY([estadoActualId]) REFERENCES [dbo].[EstadosTramites]([id]),
    CONSTRAINT [Tramites_tipoTramiteId_fk] FOREIGN KEY([tipoTramiteId]) REFERENCES [dbo].[TipoTramite]([id]),
    CONSTRAINT [Tramites_usuarioId_fk] FOREIGN KEY([usuarioId]) REFERENCES [dbo].[Usuarios]([id])
);

CREATE TABLE [dbo].[ConceptosTarifaria] (
    [id] int NOT NULL IDENTITY(1,1),
    [nombre] nvarchar(70) NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    [temaId] int NOT NULL,
    PRIMARY KEY ([id]),
    CONSTRAINT [ConceptosTarifaria_temaId_fk] FOREIGN KEY([temaId]) REFERENCES [dbo].[TemasTarifaria]([id])
);

CREATE TABLE [dbo].[Secciones] (
    [id] int NOT NULL IDENTITY(1,1),
    [nombre] nvarchar(50) NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    [filas] int NOT NULL,
    [nroParcelas] int NOT NULL,
    [tipoNumeracionParcelaId] int NOT NULL,
    [tipoParcelaId] int NOT NULL,
    PRIMARY KEY ([id]),
    CONSTRAINT [Secciones_tipoNumeracionParcelaId_fk] FOREIGN KEY([tipoNumeracionParcelaId]) REFERENCES [dbo].[TipoNumeracionParcelas]([id]),
    CONSTRAINT [Secciones_tipoParcelaId_fk] FOREIGN KEY([tipoParcelaId]) REFERENCES [dbo].[TipoParcela]([id])
);

CREATE TABLE [dbo].[Notas] (
    [tramiteId] int Primary key NOT NULL,
    [nombre] nvarchar(100) NOT NULL,
    [tipoNotaId] int NOT NULL,
    [descripcion] nvarchar(max),
    [color] nvarchar(16),
	visibilidad bit NOT NULL DEFAULT 1,
	tramiteIngresoId int null,
	FechaFinRecordatorio datetime2 null,
	foreign key (tramiteId) references Tramites(id),
	foreign key (tramiteIngresoId) references Tramites(id),
    CONSTRAINT [Notas_tipoNotaId_fk] FOREIGN KEY([tipoNotaId]) REFERENCES [dbo].[TipoNota]([id])
);
-- Tablas con dependencias de segundo nivel
CREATE TABLE [dbo].[Parcelas] (
    [id] int NOT NULL IDENTITY(1,1),
    [visibilidad] bit NOT NULL DEFAULT 1,
    [nroParcela] int NOT NULL,
    [nroFila] int NOT NULL,
    [cantidadDifuntos] int NOT NULL,
    [nombrePanteon] nvarchar(50) NOT NULL,
    [informacionAdicional] nvarchar(max) NOT NULL,
    [seccionId] int NOT NULL,
    [tipoNichoId] int,
    [tipoPanteonId] int,
    [tipoParcelaId] int,
    PRIMARY KEY ([id]),
    CONSTRAINT [Parcelas_seccionId_fk] FOREIGN KEY([seccionId]) REFERENCES [dbo].[Secciones]([id]),
    CONSTRAINT [Parcelas_tipoNichoId_fk] FOREIGN KEY([tipoNichoId]) REFERENCES [dbo].[TipoNichos]([id]),
    CONSTRAINT [Parcelas_tipoPanteonId_fk] FOREIGN KEY([tipoPanteonId]) REFERENCES [dbo].[TipoPanteon]([id]),
    CONSTRAINT [Parcelas_tipoParcelaId_fk] FOREIGN KEY([tipoParcelaId]) REFERENCES [dbo].[TipoParcela]([id])
);

CREATE TABLE [dbo].[PreciosTarifarias] (
    [id] int NOT NULL IDENTITY(1,1),
    [precio] decimal(15, 2) NOT NULL,
    [nroFila] int,
    [conceptoTarifariaId] int NOT NULL,
    [aniosConcesionId] int,
	[visibilidad] bit default 1,
    [seccionId] int,
    PRIMARY KEY ([id]),
    CONSTRAINT [PreciosTarifarias_conceptoTarifariaId_fk] FOREIGN KEY([conceptoTarifariaId]) REFERENCES [dbo].[ConceptosTarifaria]([id]),
    CONSTRAINT [PreciosTarifarias_seccionId_fk] FOREIGN KEY([seccionId]) REFERENCES [dbo].[Secciones]([id])
);

CREATE TABLE [dbo].[Tareas] (
    [id] int NOT NULL IDENTITY(1,1),
    [estado] bit NOT NULL,
    [descripcion] nvarchar(60) NOT NULL,
    [notaId] int,
    [tramiteId] int,
	[visibilidad] bit not null default 1,
    PRIMARY KEY ([id]),
    CONSTRAINT [Tareas_notaId_fk] FOREIGN KEY([notaId]) REFERENCES [dbo].[Notas]([tramiteId]),
    CONSTRAINT [Tareas_tramiteId_fk] FOREIGN KEY([tramiteId]) REFERENCES [dbo].[Tramites]([id])
);


-- Tablas con dependencias de tercer nivel
CREATE TABLE [dbo].[Archivos] (
    [id] UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL UNIQUE DEFAULT NEWID(),
    [categoriaArchivo] nvarchar(50),
    [tramiteId] int,
    [nombreArchivo] nvarchar(255) NOT NULL,
    [tipoArchivo] nvarchar(50) NOT NULL,
    [tamanoBytes] bigint NOT NULL,
    [contenido] VARBINARY(MAX) FILESTREAM NOT NULL,
    [descripcion] nvarchar(255),
    [fechaCreacion] DATETIME2 DEFAULT SYSDATETIME(),
    [visibilidad] bit NOT NULL DEFAULT 1,
    PRIMARY KEY ([id]),
    CONSTRAINT [Archivos_tramiteId_fk] FOREIGN KEY([tramiteId])
        REFERENCES [dbo].[Tramites]([id])
);

CREATE TABLE [dbo].[HistorialEstadoTramite] (
    [id] int NOT NULL IDENTITY(1,1),
    [fecha] datetime NOT NULL,
    [tramiteId] int NOT NULL,
    [estadoTramiteId] int NOT NULL,
    PRIMARY KEY ([id]),
    CONSTRAINT [HistorialEstadoTramite_estadoTramiteId_fk] FOREIGN KEY([estadoTramiteId]) REFERENCES [dbo].[EstadosTramites]([id]),
    CONSTRAINT [HistorialEstadoTramite_tramiteId_fk] FOREIGN KEY([tramiteId]) REFERENCES [dbo].[Tramites]([id])
);

CREATE TABLE [dbo].[TramitePersona] (
    [tramiteId] int NOT NULL,
    [personaId] int NOT NULL,
    [fechaRegistro] datetime NOT NULL,
    PRIMARY KEY ([tramiteId], [personaId]),
    CONSTRAINT [TramitePersona_personaId_fk] FOREIGN KEY([personaId]) REFERENCES [dbo].[Personas]([id]),
    CONSTRAINT [TramitePersona_tramiteId_fk] FOREIGN KEY([tramiteId]) REFERENCES [dbo].[Tramites]([id])
);

CREATE TABLE [dbo].[TramitesParcela] (
    [tramiteId] int NOT NULL,
    [parcelaId] int NOT NULL,
    [fechaRegistro] datetime2 NOT NULL,
    PRIMARY KEY ([tramiteId], [parcelaId]),
    CONSTRAINT [TramitesParcela_parcelaId_fk] FOREIGN KEY([parcelaId]) REFERENCES [dbo].[Parcelas]([id]),
    CONSTRAINT [TramitesParcela_tramiteId_fk] FOREIGN KEY([tramiteId]) REFERENCES [dbo].[Tramites]([id])
);

CREATE TABLE [dbo].[Introducciones] (
    [tramiteId] int NOT NULL,
    [visibilidad] bit NOT NULL DEFAULT 1,
    [fechaIngreso] datetime,
    [usuarioId] int NOT NULL,
    [empresaFunebreId] int,
    [parcelaId] int NOT NULL,
    [difuntoId] int NOT NULL,
    [estadoDifuntoId] int not null,
    [informacionAdicional] nvarchar(max),
    PRIMARY KEY ([tramiteId]),
	foreign key (estadoDifuntoId) references EstadosDifunto(id),
    CONSTRAINT [Introducciones_difuntoId_fk] FOREIGN KEY([difuntoId]) REFERENCES [dbo].[Personas]([id]),
    CONSTRAINT [Introducciones_empresaFunebreId_fk] FOREIGN KEY([empresaFunebreId]) REFERENCES [dbo].[EmpresasFunebres]([id]),
    CONSTRAINT [Introducciones_parcelaId_fk] FOREIGN KEY([parcelaId]) REFERENCES [dbo].[Parcelas]([id]),
    CONSTRAINT [Introducciones_tramiteId_fk] FOREIGN KEY([tramiteId]) REFERENCES [dbo].[Tramites]([id]),
    CONSTRAINT [Introducciones_usuarioId_fk] FOREIGN KEY([usuarioId]) REFERENCES [dbo].[Usuarios]([id])
);

CREATE TABLE [dbo].[ParcelaDifuntos] (
    [id] int NOT NULL IDENTITY(1,1),
    [parcelaId] int NOT NULL,
    [difuntoId] int NOT NULL,
    [fechaIngreso] datetime,
    [fechaRetiro] datetime,
    [tramiteIngresoId] int,
    [tramiteRetiroId] int,
    PRIMARY KEY ([id]),
    CONSTRAINT [ParcelaDifuntos_difuntoId_fk] FOREIGN KEY([difuntoId]) REFERENCES [dbo].[Personas]([id]),
    CONSTRAINT [ParcelaDifuntos_parcelaId_fk] FOREIGN KEY([parcelaId]) REFERENCES [dbo].[Parcelas]([id]),
    CONSTRAINT [ParcelaDifuntos_tramiteIngresoId_fk] FOREIGN KEY([tramiteIngresoId]) REFERENCES [dbo].[Tramites]([id]),
    CONSTRAINT [ParcelaDifuntos_tramiteRetiroId_fk] FOREIGN KEY([tramiteRetiroId]) REFERENCES [dbo].[Tramites]([id])
);

CREATE TABLE [dbo].[Concesiones] (
    [tramiteId] int NOT NULL,
    [concesion] int,
    [precio] decimal(15, 2),
    [visibilidad] bit DEFAULT 1,
	[informacionAdicional] nvarchar(max),
    [tipoParcela] nvarchar(20),
    [vencimiento] date,
    [parcelaId] int NOT NULL,
    [cantidadAniosId] int,
    [cuotaId] int,
    [usuarioId] int,
    PRIMARY KEY ([tramiteId]),
    CONSTRAINT [Concesiones_cantidadAniosId_fk] FOREIGN KEY([cantidadAniosId]) REFERENCES [dbo].[AnioConcesion]([id]),
    CONSTRAINT [Concesiones_cuotaId_fk] FOREIGN KEY([cuotaId]) REFERENCES [dbo].[CantidadCuotas]([id]),
    CONSTRAINT [Concesiones_parcelaId_fk] FOREIGN KEY([parcelaId]) REFERENCES [dbo].[Parcelas]([id]),
    CONSTRAINT [Concesiones_tramiteId_fk] FOREIGN KEY([tramiteId]) REFERENCES [dbo].[Tramites]([id]),
    CONSTRAINT [Concesiones_usuarioId_fk] FOREIGN KEY([usuarioId]) REFERENCES [dbo].[Usuarios]([id])
);

CREATE TABLE [dbo].[HistorialTitularesConcesiones] (
    [id] int NOT NULL IDENTITY(1,1),
    [concesionId] int,
    [personaId] int,
    [fechaInicio] datetime,
    [fechaFin] datetime,
    PRIMARY KEY ([id]),
    CONSTRAINT [HistorialTitularesConcesiones_concesionId_fk] FOREIGN KEY([concesionId]) REFERENCES [dbo].[Concesiones]([tramiteId]),
    CONSTRAINT [HistorialTitularesConcesiones_personaId_fk] FOREIGN KEY([personaId]) REFERENCES [dbo].[Personas]([id])
);

CREATE TABLE ReglasIngreso (
    id INT IDENTITY PRIMARY KEY,
	nombreRegla nvarchar(100) not null,
    -- CONDICIONES
    tipoParcelaId INT NOT NULL,
    estadoDifuntoId INT NOT NULL,
    tipoNichoId INT NULL,
    tipoPanteonId INT NULL,
    -- RESULTADO (todos ConceptosTarifaria)
    conceptoInhumacionId INT NOT NULL,
    conceptoDefuncionId INT NOT NULL,
    conceptoTranscripcionId INT NULL,
    conceptoIntroduccionId INT NOT NULL,
    porcentajeFondoSaludId INT NOT NULL,
    porcentajeAumentoOtraLocalidadId INT NULL,
    porcentajeAumentoDerechoOficinaId INT NULL,
    porcentajeIntroduccionUrnaDerechoOficna INT NULL,
    montoMinimoFondoId INT NULL,
    visibilidad BIT NOT NULL DEFAULT 1,
	cierreNicho int null,
	cierreFosa int null
     -- FOREIGN KEYS (TODAS CON NOMBRES ÚNICOS)
    CONSTRAINT FK_RI_TipoParcela 
        FOREIGN KEY (tipoParcelaId) REFERENCES TipoParcela(id),

    CONSTRAINT FK_RI_EstadoDifunto 
        FOREIGN KEY (estadoDifuntoId) REFERENCES EstadosDifunto(id),

    CONSTRAINT FK_RI_TipoNicho 
        FOREIGN KEY (tipoNichoId) REFERENCES TipoNichos(id),

    CONSTRAINT FK_RI_TipoPanteon 
        FOREIGN KEY (tipoPanteonId) REFERENCES TipoPanteon(id),

    CONSTRAINT FK_RI_ConceptoInhumacion 
        FOREIGN KEY (conceptoInhumacionId) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_ConceptoDefuncion 
        FOREIGN KEY (conceptoDefuncionId) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_ConceptoTranscripcion 
        FOREIGN KEY (conceptoTranscripcionId) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_ConceptoIntroduccion 
        FOREIGN KEY (conceptoIntroduccionId) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_PorcFondoSalud 
        FOREIGN KEY (porcentajeFondoSaludId) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_PorcAumentoOtraLocalidad 
        FOREIGN KEY (porcentajeAumentoOtraLocalidadId) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_PorcAumentoDerechoOficina 
        FOREIGN KEY (porcentajeAumentoDerechoOficinaId) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_PorcIntroduccionUrnaDO 
        FOREIGN KEY (porcentajeIntroduccionUrnaDerechoOficna) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_MontoMinimoFondo 
        FOREIGN KEY (montoMinimoFondoId) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_CierreNicho 
        FOREIGN KEY (cierreNicho) REFERENCES ConceptosTarifaria(id),

    CONSTRAINT FK_RI_CierreFosa 
        FOREIGN KEY (cierreFosa) REFERENCES ConceptosTarifaria(id)
);
