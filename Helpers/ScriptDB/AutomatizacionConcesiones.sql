/* ======================================================================
   CEMSYS - Actualización automática de estado de Concesiones
   Vigente (6) -> Vencido (7) cuando la fecha de vencimiento ya pasó
   ======================================================================
   ANTES DE EJECUTAR:
   Verificá que los ids de tu enum EstadosConcesionEnum coincidan con
   las filas reales de EstadosTramites para el tipo de trámite Concesión:

       SELECT et.id, et.estado, et.tipoTramiteId, tt.tipo
       FROM EstadosTramites et
       JOIN TipoTramite tt ON tt.id = et.tipoTramiteId
       WHERE et.id IN (5,6,7,8);

   Si no coinciden, ajustá las variables @EstadoVigente / @EstadoVencido
   dentro del procedimiento más abajo.
   ====================================================================== */

USE cemsys;
GO

/* ======================================================================
   1) Tabla de control: guarda si el proceso ya corrió "hoy"
      Sirve para:
      - No reprocesar innecesariamente varias veces en el mismo día
      - Saber, ante un corte de luz, si el día ya quedó cubierto o no
   ====================================================================== */


/* ======================================================================
   2) Procedimiento principal
      - Recorre Tramites/Concesiones en estado Vigente
      - Si vencimiento < hoy -> pasa a Vencido y graba en HistorialEstadoTramite
      - Si vencimiento >= hoy -> lo deja como está
      - Si ya está Vencido -> no lo toca (el WHERE ya lo excluye)
      - @Forzar = 1 : lo usa el botón manual del sistema, para poder
        disparar la revisión aunque el proceso ya haya corrido hoy.
        No genera duplicados: una vez que un trámite pasa a Vencido,
        deja de cumplir la condición "estadoActualId = Vigente", así
        que reejecutar es seguro (idempotente).
   ====================================================================== */



/* ======================================================================
   3) SQL Server Agent: creación del Job con 3 horarios
      a) Diario a las 00:00
      b) Al iniciar el SQL Server Agent (cubre reinicios por corte de luz)
      c) Reintento cada 1 hora entre 06:00 y 14:00, como red de seguridad
         (gracias al control interno del SP, si ya corrió hoy no hace nada,
          así que este horario extra no genera duplicados)

      REQUIERE que el servicio "SQL Server Agent" esté instalado y
      corriendo. Esto NO está disponible en SQL Server Express (ver nota
      al final).
   ====================================================================== */



/* ======================================================================
   4) Botón manual dentro del sistema
      Simplemente ejecutá, desde tu backend, con @Forzar = 1:

          EXEC dbo.sp_ActualizarEstadoConcesiones @Forzar = 1;

      Es seguro llamarlo aunque ya haya corrido el job automático ese día:
      el WHERE del procedimiento solo toca trámites que TODAVÍA están en
      Vigente, así que no puede duplicar cambios ni historial.
   ====================================================================== */

USE cemsys;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ActualizarEstadoConcesiones
    @Forzar BIT = 0,
    @CantidadActualizadas INT = 0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @NombreProceso NVARCHAR(100) = 'ActualizarEstadoConcesiones';
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
    DECLARE @EstadoVigente INT = 6;
    DECLARE @EstadoVencido INT = 7;
    DECLARE @LockResult INT;

    EXEC @LockResult = sp_getapplock
        @Resource   = 'sp_ActualizarEstadoConcesiones',
        @LockMode   = 'Exclusive',
        @LockOwner  = 'Session',
        @LockTimeout = 5000;

    IF @LockResult < 0
    BEGIN
        PRINT 'Ya hay otra ejecución en curso, se aborta esta.';
        SET @CantidadActualizadas = 0;
        RETURN;
    END

    IF @Forzar = 0 AND EXISTS (
        SELECT 1 FROM dbo.ControlProcesosDiarios
        WHERE nombreProceso = @NombreProceso AND fechaEjecucion = @Hoy
    )
    BEGIN
        PRINT 'El proceso ya se ejecutó hoy. No se realizan cambios.';
        SET @CantidadActualizadas = 0;
        EXEC sp_releaseapplock @Resource = 'sp_ActualizarEstadoConcesiones', @LockOwner = 'Session';
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @AVencer TABLE (tramiteId INT PRIMARY KEY);

        INSERT INTO @AVencer (tramiteId)
        SELECT t.id
        FROM dbo.Tramites t
        INNER JOIN dbo.Concesiones c ON c.tramiteId = t.id
        WHERE t.estadoActualId = @EstadoVigente
          AND c.vencimiento < @Hoy
          AND t.visibilidad = 1
          AND ISNULL(c.visibilidad, 1) = 1;

        INSERT INTO dbo.HistorialEstadoTramite (fecha, tramiteId, estadoTramiteId)
        SELECT GETDATE(), tramiteId, @EstadoVencido
        FROM @AVencer;

        UPDATE t
            SET t.estadoActualId = @EstadoVencido
        FROM dbo.Tramites t
        INNER JOIN @AVencer v ON v.tramiteId = t.id;

        DECLARE @Cantidad INT = (SELECT COUNT(*) FROM @AVencer);
        SET @CantidadActualizadas = @Cantidad;

        IF EXISTS (SELECT 1 FROM dbo.ControlProcesosDiarios WHERE nombreProceso = @NombreProceso AND fechaEjecucion = @Hoy)
            UPDATE dbo.ControlProcesosDiarios
                SET fechaHoraEjecucion = GETDATE(),
                    cantidadActualizadas = ISNULL(cantidadActualizadas, 0) + @Cantidad
            WHERE nombreProceso = @NombreProceso AND fechaEjecucion = @Hoy;
        ELSE
            INSERT INTO dbo.ControlProcesosDiarios (nombreProceso, fechaEjecucion, fechaHoraEjecucion, cantidadActualizadas)
            VALUES (@NombreProceso, @Hoy, GETDATE(), @Cantidad);

        COMMIT TRANSACTION;

        PRINT CONCAT('Proceso OK. Concesiones pasadas a Vencido: ', @Cantidad);
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        EXEC sp_releaseapplock @Resource = 'sp_ActualizarEstadoConcesiones', @LockOwner = 'Session';
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @Sev INT = ERROR_SEVERITY();
        DECLARE @St INT = ERROR_STATE();
        RAISERROR(@Msg, @Sev, @St);
        RETURN;
    END CATCH

    EXEC sp_releaseapplock @Resource = 'sp_ActualizarEstadoConcesiones', @LockOwner = 'Session';
END
GO
