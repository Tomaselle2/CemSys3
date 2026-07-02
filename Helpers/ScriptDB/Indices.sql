USE cemsys;
GO
 
-- ============================================================================
-- TABLAS "CATALOGO" (RolesUsuarios, TipoTramite, etc.)
-- No tienen FK entrantes relevantes ni aparecen filtradas en las consultas
-- provistas -> no necesitan índices adicionales más allá de su PK.
-- ============================================================================
 
-- ============================================================================
-- Usuarios
-- ============================================================================
CREATE INDEX IX_Usuarios_RolId ON dbo.Usuarios(rolId);
-- Login / búsqueda por usuario: no vino en las consultas provistas, pero es
-- un acceso casi seguro en cualquier sistema con tabla Usuarios. Si hacés
-- login por "usuario" o "correo", estos índices lo aceleran mucho.
CREATE UNIQUE INDEX IX_Usuarios_Usuario ON dbo.Usuarios(usuario) WHERE visibilidad = 1;
CREATE INDEX IX_Usuarios_Correo ON dbo.Usuarios(correo);
 
-- ============================================================================
-- Personas
-- ============================================================================
CREATE INDEX IX_Personas_CategoriaPersonaId ON dbo.Personas(categoriaPersonaId);
CREATE INDEX IX_Personas_EstadoDifuntoId ON dbo.Personas(estadoDifuntoId);
-- GetAllFiltro filtra por Dni exacto (equality) -> muy útil.
CREATE INDEX IX_Personas_Dni ON dbo.Personas(dni);
-- GetAllFiltro hace .Contains(nombre)/.Contains(apellido) (LIKE '%x%'), que
-- no puede hacer seek, pero el ORDER BY Apellido, Nombre + el SELECT de
-- columnas puntuales se benefician de un índice cubriente para evitar
-- lecturas de la tabla base fila por fila.
CREATE INDEX IX_Personas_Apellido_Nombre
    ON dbo.Personas(apellido, nombre)
    INCLUDE (dni, sexo, visibilidad, categoriaPersonaId);
 
-- ============================================================================
-- EstadosTramites
-- ============================================================================
CREATE INDEX IX_EstadosTramites_TipoTramiteId ON dbo.EstadosTramites(tipoTramiteId);
 
-- ============================================================================
-- Tramites
-- ============================================================================
-- EstadoActualId es, con diferencia, la FK más consultada: aparece en JOIN
-- desde Cremaciones, Traslados, Reducciones, Introducciones, CambiosTitularidad,
-- AceptacionTitularidad, PermisosIngresos, PermisosRefacciones y en el
-- filtro por estado del listado de Concesiones.
CREATE INDEX IX_Tramites_EstadoActualId
    ON dbo.Tramites(estadoActualId)
    INCLUDE (tipoTramiteId, fechaCreacion, usuarioId);
CREATE INDEX IX_Tramites_TipoTramiteId ON dbo.Tramites(tipoTramiteId);
CREATE INDEX IX_Tramites_UsuarioId ON dbo.Tramites(usuarioId);
-- Se usa mucho OrderByDescending(FechaCreacion) sobre listas ya combinadas.
CREATE INDEX IX_Tramites_FechaCreacion ON dbo.Tramites(fechaCreacion DESC);
 
-- ============================================================================
-- ConceptosTarifaria
-- ============================================================================
CREATE INDEX IX_ConceptosTarifaria_TemaId ON dbo.ConceptosTarifaria(temaId);
 
-- ============================================================================
-- Secciones
-- ============================================================================
CREATE INDEX IX_Secciones_TipoNumeracionParcelaId ON dbo.Secciones(tipoNumeracionParcelaId);
CREATE INDEX IX_Secciones_TipoParcelaId ON dbo.Secciones(tipoParcelaId);
 
-- ============================================================================
-- Notas
-- ============================================================================
CREATE INDEX IX_Notas_TipoNotaId ON dbo.Notas(tipoNotaId);
CREATE INDEX IX_Notas_TramiteIngresoId ON dbo.Notas(tramiteIngresoId);
 
-- ============================================================================
-- Parcelas
-- ============================================================================
-- SeccionId se usa como filtro directo (seccionID) y a través de
-- c.Parcela.SeccionId en el JOIN de Concesiones.
CREATE INDEX IX_Parcelas_SeccionId
    ON dbo.Parcelas(seccionId)
    INCLUDE (tipoParcelaId, nroParcela, nroFila, nombrePanteon);
CREATE INDEX IX_Parcelas_TipoParcelaId ON dbo.Parcelas(tipoParcelaId);
CREATE INDEX IX_Parcelas_TipoNichoId ON dbo.Parcelas(tipoNichoId);
CREATE INDEX IX_Parcelas_TipoPanteonId ON dbo.Parcelas(tipoPanteonId);
 
-- ============================================================================
-- PreciosTarifarias
-- ============================================================================
CREATE INDEX IX_PreciosTarifarias_ConceptoTarifariaId ON dbo.PreciosTarifarias(conceptoTarifariaId);
CREATE INDEX IX_PreciosTarifarias_SeccionId ON dbo.PreciosTarifarias(seccionId);
-- GetPrecios() filtra siempre por Visibilidad = true -> índice filtrado.
CREATE INDEX IX_PreciosTarifarias_Visibilidad_Activos
    ON dbo.PreciosTarifarias(visibilidad)
    INCLUDE (conceptoTarifariaId, aniosConcesionId, seccionId, precio, nroFila)
    WHERE visibilidad = 1;
 
-- ============================================================================
-- TareaPlantilla
-- ============================================================================
CREATE INDEX IX_TareaPlantilla_TipoTramiteId ON dbo.TareaPlantilla(TipoTramiteId);
 
-- ============================================================================
-- Tareas
-- ============================================================================
-- GetAllByTramite(tramiteId) filtra directo por TramiteId.
CREATE INDEX IX_Tareas_TramiteId
    ON dbo.Tareas(tramiteId)
    INCLUDE (estado, descripcion, notaId, tareaPlantillaId, visibilidad);
CREATE INDEX IX_Tareas_NotaId ON dbo.Tareas(notaId);
CREATE INDEX IX_Tareas_TareaPlantillaId ON dbo.Tareas(TareaPlantillaId);
 
-- ============================================================================
-- Archivos
-- ============================================================================
-- GetAllByTramiteId ordena por FechaCreacion DESC filtrando por TramiteId.
CREATE INDEX IX_Archivos_TramiteId_FechaCreacion
    ON dbo.Archivos(tramiteId, fechaCreacion DESC)
    INCLUDE (categoriaArchivo, nombreArchivo, tipoArchivo, descripcion, visibilidad);
 
-- ============================================================================
-- HistorialEstadoTramite
-- ============================================================================
CREATE INDEX IX_HistorialEstadoTramite_TramiteId ON dbo.HistorialEstadoTramite(tramiteId);
CREATE INDEX IX_HistorialEstadoTramite_EstadoTramiteId ON dbo.HistorialEstadoTramite(estadoTramiteId);
 
-- ============================================================================
-- TramitePersona (PK compuesta tramiteId+personaId ya cubre tramiteId)
-- ============================================================================
CREATE INDEX IX_TramitePersona_PersonaId ON dbo.TramitePersona(personaId);
 
-- ============================================================================
-- TramitesParcela (PK compuesta tramiteId+parcelaId ya cubre tramiteId)
-- ============================================================================
-- GetListadoTramitesDeConcesion filtra por ParcelaId y ordena por FechaRegistro DESC.
CREATE INDEX IX_TramitesParcela_ParcelaId_FechaRegistro
    ON dbo.TramitesParcela(parcelaId, fechaRegistro DESC)
    INCLUDE (tramiteId);
 
-- ============================================================================
-- Introducciones (PK = tramiteId)
-- ============================================================================
CREATE INDEX IX_Introducciones_UsuarioId ON dbo.Introducciones(usuarioId);
CREATE INDEX IX_Introducciones_EmpresaFunebreId ON dbo.Introducciones(empresaFunebreId);
CREATE INDEX IX_Introducciones_ParcelaId ON dbo.Introducciones(parcelaId);
CREATE INDEX IX_Introducciones_DifuntoId ON dbo.Introducciones(difuntoId);
CREATE INDEX IX_Introducciones_EstadoDifuntoId ON dbo.Introducciones(estadoDifuntoId);
 
-- ============================================================================
-- ParcelaDifuntos
-- ============================================================================
-- Patrón muy repetido en tu código: Where(pd => parcelaIds.Contains(pd.ParcelaId)
-- && pd.FechaRetiro == null) -> "difuntos actuales de la parcela".
-- Índice filtrado: mucho más chico y rápido que uno sobre toda la tabla.
CREATE INDEX IX_ParcelaDifuntos_ParcelaId_Activos
    ON dbo.ParcelaDifuntos(parcelaId)
    INCLUDE (difuntoId, fechaIngreso)
    WHERE fechaRetiro IS NULL;
CREATE INDEX IX_ParcelaDifuntos_DifuntoId ON dbo.ParcelaDifuntos(difuntoId);
CREATE INDEX IX_ParcelaDifuntos_TramiteIngresoId ON dbo.ParcelaDifuntos(tramiteIngresoId);
CREATE INDEX IX_ParcelaDifuntos_TramiteRetiroId ON dbo.ParcelaDifuntos(tramiteRetiroId);
 
-- ============================================================================
-- Concesiones (PK = tramiteId)
-- ============================================================================
-- Filtros usados en GellAllPaginado: ParcelaId directo, Concesion (número),
-- Vencimiento (rango de fechas), y FechaFin == null para "concesión activa"
-- (se usa junto con ParcelaDifuntos en varias subconsultas).
CREATE INDEX IX_Concesiones_ParcelaId_FechaFin
    ON dbo.Concesiones(parcelaId, fechaFin)
    INCLUDE (vencimiento, concesion, visibilidad);
CREATE INDEX IX_Concesiones_Concesion ON dbo.Concesiones(concesion);
CREATE INDEX IX_Concesiones_Vencimiento ON dbo.Concesiones(vencimiento);
CREATE INDEX IX_Concesiones_CantidadAniosId ON dbo.Concesiones(cantidadAniosId);
CREATE INDEX IX_Concesiones_CuotaId ON dbo.Concesiones(cuotaId);
CREATE INDEX IX_Concesiones_UsuarioId ON dbo.Concesiones(usuarioId);
CREATE INDEX IX_Concesiones_TramiteRetiroId ON dbo.Concesiones(TramiteRetiroId);
 
-- ============================================================================
-- HistorialTitularesConcesiones
-- ============================================================================
-- Where(h => tramiteIds.Contains(h.ConcesionId) && h.FechaFin == null)
-- para obtener el/los titular(es) vigente(s) de cada concesión.
CREATE INDEX IX_HistorialTitularesConcesiones_ConcesionId_FechaFin
    ON dbo.HistorialTitularesConcesiones(concesionId, fechaFin)
    INCLUDE (personaId);
CREATE INDEX IX_HistorialTitularesConcesiones_PersonaId ON dbo.HistorialTitularesConcesiones(personaId);
 
-- ============================================================================
-- ReglasIngreso
-- ============================================================================
-- La lógica de negocio típica busca la regla que matchea una combinación
-- de tipoParcela/estadoDifunto/tipoNicho/tipoPanteon.
CREATE INDEX IX_ReglasIngreso_Combinacion
    ON dbo.ReglasIngreso(tipoParcelaId, estadoDifuntoId, tipoNichoId, tipoPanteonId)
    INCLUDE (conceptoInhumacionId, conceptoDefuncionId, conceptoTranscripcionId,
             conceptoIntroduccionId, porcentajeFondoSaludId);
 
-- ============================================================================
-- TramitesCostos
-- ============================================================================
CREATE INDEX IX_TramitesCostos_TramiteId ON dbo.TramitesCostos(tramiteId);
CREATE INDEX IX_TramitesCostos_ConceptoTarifariaId ON dbo.TramitesCostos(conceptoTarifariaId);
 
-- ============================================================================
-- CambiosTitularidad (PK = tramiteId)
-- ============================================================================
CREATE INDEX IX_CambiosTitularidad_ParcelaId ON dbo.CambiosTitularidad(parcelaId);
CREATE INDEX IX_CambiosTitularidad_UsuarioId ON dbo.CambiosTitularidad(usuarioId);
CREATE INDEX IX_CambiosTitularidad_ConcesionId ON dbo.CambiosTitularidad(concesionId);
 
-- ============================================================================
-- TipoAutorizacion
-- ============================================================================
CREATE INDEX IX_TipoAutorizacion_TipoTramiteId ON dbo.TipoAutorizacion(tipoTramiteId);
 
-- ============================================================================
-- PlantillasTramite
-- ============================================================================
CREATE INDEX IX_PlantillasTramite_TipoTramiteId ON dbo.PlantillasTramite(tipoTramiteId);
CREATE INDEX IX_PlantillasTramite_TipoAutorizacionId ON dbo.PlantillasTramite(tipoAutorizacionId);
 
-- ============================================================================
-- DocumentosTramite
-- ============================================================================
CREATE INDEX IX_DocumentosTramite_TramiteId ON dbo.DocumentosTramite(tramiteId);
CREATE INDEX IX_DocumentosTramite_PlantillaId ON dbo.DocumentosTramite(plantillaId);
CREATE INDEX IX_DocumentosTramite_UsuarioId ON dbo.DocumentosTramite(usuarioId);
CREATE INDEX IX_DocumentosTramite_PersonaId ON dbo.DocumentosTramite(personaId);
CREATE INDEX IX_DocumentosTramite_TipoAutorizacionId ON dbo.DocumentosTramite(tipoAutorizacionId);
CREATE INDEX IX_DocumentosTramite_FirmanteId ON dbo.DocumentosTramite(firmanteId);
 
-- ============================================================================
-- RequisitosTramite
-- ============================================================================
CREATE INDEX IX_RequisitosTramite_TipoTramiteId ON dbo.RequisitosTramite(tipoTramiteId);
 
-- ============================================================================
-- AceptacionTitularidad (PK = tramiteId)
-- ============================================================================
CREATE INDEX IX_AceptacionTitularidad_ParcelaId ON dbo.AceptacionTitularidad(parcelaId);
CREATE INDEX IX_AceptacionTitularidad_UsuarioId ON dbo.AceptacionTitularidad(usuarioId);
CREATE INDEX IX_AceptacionTitularidad_ConcesionId ON dbo.AceptacionTitularidad(concesionId);
 
-- ============================================================================
-- FirmantesTramite
-- ============================================================================
CREATE INDEX IX_FirmantesTramite_TramiteId ON dbo.FirmantesTramite(tramiteId);
CREATE INDEX IX_FirmantesTramite_PersonaId ON dbo.FirmantesTramite(personaId);
 
-- ============================================================================
-- Cremaciones (PK = tramiteId)
-- ============================================================================
CREATE INDEX IX_Cremaciones_ParcelaOrigenId ON dbo.Cremaciones(parcelaOrigenId);
CREATE INDEX IX_Cremaciones_ParcelaDestinoId ON dbo.Cremaciones(parcelaDestinoId);
CREATE INDEX IX_Cremaciones_UsuarioId ON dbo.Cremaciones(usuarioId);
CREATE INDEX IX_Cremaciones_DifuntoId ON dbo.Cremaciones(difuntoId);
CREATE INDEX IX_Cremaciones_ConcesionId ON dbo.Cremaciones(concesionId);
CREATE INDEX IX_Cremaciones_CementerioId ON dbo.Cremaciones(cementerioId);
-- GetIniciadosYPendientes filtra FechaFinalizacion IS NULL + join a Tramites.
CREATE INDEX IX_Cremaciones_Activos
    ON dbo.Cremaciones(fechaFinalizacion)
    INCLUDE (tramiteId)
    WHERE fechaFinalizacion IS NULL;
 
-- ============================================================================
-- Diagramas
-- ============================================================================
-- No tiene FK declarada en el script, pero conceptualmente referencia a
-- Tramites y muy probablemente se consulta por TramiteId.
CREATE INDEX IX_Diagramas_TramiteId ON dbo.Diagramas(TramiteId);
 
-- ============================================================================
-- Traslados (PK = tramiteId) — mismo patrón que Cremaciones
-- ============================================================================
CREATE INDEX IX_Traslados_ParcelaOrigenId ON dbo.Traslados(parcelaOrigenId);
CREATE INDEX IX_Traslados_ParcelaDestinoId ON dbo.Traslados(parcelaDestinoId);
CREATE INDEX IX_Traslados_UsuarioId ON dbo.Traslados(usuarioId);
CREATE INDEX IX_Traslados_DifuntoId ON dbo.Traslados(difuntoId);
CREATE INDEX IX_Traslados_ConcesionId ON dbo.Traslados(concesionId);
CREATE INDEX IX_Traslados_CementerioId ON dbo.Traslados(cementerioId);
CREATE INDEX IX_Traslados_Activos
    ON dbo.Traslados(fechaFinalizacion)
    INCLUDE (tramiteId)
    WHERE fechaFinalizacion IS NULL;
 
-- ============================================================================
-- Reducciones (PK = tramiteId) — mismo patrón que Cremaciones
-- ============================================================================
CREATE INDEX IX_Reducciones_ParcelaOrigenId ON dbo.Reducciones(parcelaOrigenId);
CREATE INDEX IX_Reducciones_ParcelaDestinoId ON dbo.Reducciones(parcelaDestinoId);
CREATE INDEX IX_Reducciones_UsuarioId ON dbo.Reducciones(usuarioId);
CREATE INDEX IX_Reducciones_DifuntoId ON dbo.Reducciones(difuntoId);
CREATE INDEX IX_Reducciones_ConcesionId ON dbo.Reducciones(concesionId);
CREATE INDEX IX_Reducciones_CementerioId ON dbo.Reducciones(cementerioId);
CREATE INDEX IX_Reducciones_Activos
    ON dbo.Reducciones(fechaFinalizacion)
    INCLUDE (tramiteId)
    WHERE fechaFinalizacion IS NULL;
 
-- ============================================================================
-- PermisosIngresos (PK = tramiteId)
-- ============================================================================
CREATE INDEX IX_PermisosIngresos_ParcelaId ON dbo.PermisosIngresos(parcelaId);
CREATE INDEX IX_PermisosIngresos_UsuarioId ON dbo.PermisosIngresos(usuarioId);
CREATE INDEX IX_PermisosIngresos_ConcesionId ON dbo.PermisosIngresos(concesionId);
 
-- ============================================================================
-- PermisosRefacciones (PK = tramiteId)
-- ============================================================================
CREATE INDEX IX_PermisosRefacciones_ParcelaId ON dbo.PermisosRefacciones(parcelaId);
CREATE INDEX IX_PermisosRefacciones_UsuarioId ON dbo.PermisosRefacciones(usuarioId);
CREATE INDEX IX_PermisosRefacciones_ConcesionId ON dbo.PermisosRefacciones(concesionId);
-- GetIniciadosYPendientes: Where(FechaFinalizacion == null && EstadoActual in (...))
CREATE INDEX IX_PermisosRefacciones_Activos
    ON dbo.PermisosRefacciones(fechaFinalizacion)
    INCLUDE (tramiteId)
    WHERE fechaFinalizacion IS NULL;
 
-- ============================================================================
-- EventoCalendario
-- ============================================================================
-- Un calendario casi siempre se consulta por rango de fechas.
CREATE INDEX IX_EventoCalendario_Fecha ON dbo.EventoCalendario(Fecha);
 
-- ============================================================================
-- HistorialParcelasConcesion
-- ============================================================================
CREATE INDEX IX_HistorialParcelasConcesion_ConcesionId ON dbo.HistorialParcelasConcesion(concesionId);
-- Patrón esperable: "parcela actual de una concesión" = FechaFin IS NULL.
CREATE INDEX IX_HistorialParcelasConcesion_ParcelaId_Activos
    ON dbo.HistorialParcelasConcesion(parcelaId, fechaFin)
    INCLUDE (concesionId, fechaInicio, tramiteOrigenId);
CREATE INDEX IX_HistorialParcelasConcesion_TramiteOrigenId ON dbo.HistorialParcelasConcesion(tramiteOrigenId);
 