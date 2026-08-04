use cemsys
go
/* ======================================================================
   1) Tabla de control: guarda si el proceso ya corrió "hoy"
      Sirve para:
      - No reprocesar innecesariamente varias veces en el mismo día
      - Saber, ante un corte de luz, si el día ya quedó cubierto o no
   ====================================================================== */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ControlProcesosDiarios')
BEGIN
    CREATE TABLE dbo.ControlProcesosDiarios (
        [id] INT IDENTITY(1,1) NOT NULL,
        [nombreProceso] NVARCHAR(100) NOT NULL,
        [fechaEjecucion] DATE NOT NULL,           -- día "cubierto"
        [fechaHoraEjecucion] DATETIME NOT NULL DEFAULT GETDATE(),
        [cantidadActualizadas] INT NULL,
        PRIMARY KEY ([id]),
        CONSTRAINT [UQ_ControlProcesosDiarios] UNIQUE ([nombreProceso], [fechaEjecucion])
    );
END
GO
 
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
CREATE OR ALTER PROCEDURE dbo.sp_ActualizarEstadoConcesiones
    @Forzar BIT = 0,
    @CantidadActualizadas INT = 0 OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
 
    DECLARE @NombreProceso NVARCHAR(100) = 'ActualizarEstadoConcesiones';
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
    DECLARE @EstadoVigente INT = 6;  -- EstadosConcesionEnum.Vigente
    DECLARE @EstadoVencido INT = 7;  -- EstadosConcesionEnum.Vencido
    DECLARE @LockResult INT;
 
    -- Evita que dos disparadores (job programado + arranque + botón manual)
    -- corran al mismo tiempo y pisen resultados.
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
 
    -- Si no se fuerza y ya corrió hoy, no hace nada más.
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
 
        -- Historial del cambio de estado (uno por trámite afectado)
        INSERT INTO dbo.HistorialEstadoTramite (fecha, tramiteId, estadoTramiteId)
        SELECT GETDATE(), tramiteId, @EstadoVencido
        FROM @AVencer;
 
        -- Actualiza el estado actual del trámite
        UPDATE t
            SET t.estadoActualId = @EstadoVencido
        FROM dbo.Tramites t
        INNER JOIN @AVencer v ON v.tramiteId = t.id;
 
        DECLARE @Cantidad INT = (SELECT COUNT(*) FROM @AVencer);
        SET @CantidadActualizadas = @Cantidad;
 
        -- Registra/actualiza el control de ejecución del día
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
USE msdb;
GO
 
IF EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = 'CEMSYS - Actualizar Estado Concesiones')
    EXEC msdb.dbo.sp_delete_job @job_name = 'CEMSYS - Actualizar Estado Concesiones';
GO
 
EXEC msdb.dbo.sp_add_job
    @job_name = 'CEMSYS - Actualizar Estado Concesiones',
    @enabled = 1,
    @description = 'Pasa las concesiones Vigentes vencidas a estado Vencido y registra el cambio en HistorialEstadoTramite.';
GO
 
EXEC msdb.dbo.sp_add_jobstep
    @job_name = 'CEMSYS - Actualizar Estado Concesiones',
    @step_name = 'Ejecutar sp_ActualizarEstadoConcesiones',
    @subsystem = 'TSQL',
    @database_name = 'cemsys',
    @command = 'EXEC dbo.sp_ActualizarEstadoConcesiones;',
    @on_success_action = 1,   -- Quit with success
    @on_fail_action = 2,      -- Quit with failure
    @retry_attempts = 3,
    @retry_interval = 5;      -- minutos
GO
 
-- a) Diario a las 00:00
EXEC msdb.dbo.sp_add_schedule
    @schedule_name = 'CEMSYS - Diario 00:00',
    @freq_type = 4,             -- diario
    @freq_interval = 1,
    @active_start_time = 000000;
GO
 
EXEC msdb.dbo.sp_attach_schedule
    @job_name = 'CEMSYS - Actualizar Estado Concesiones',
    @schedule_name = 'CEMSYS - Diario 00:00';
GO
 
-- b) Al iniciar el SQL Server Agent (cubre el caso de corte de luz)
EXEC msdb.dbo.sp_add_schedule
    @schedule_name = 'CEMSYS - Al iniciar el Agente',
    @freq_type = 64;            -- Start automatically when SQL Server Agent starts
GO
 
EXEC msdb.dbo.sp_attach_schedule
    @job_name = 'CEMSYS - Actualizar Estado Concesiones',
    @schedule_name = 'CEMSYS - Al iniciar el Agente';
GO
 
-- c) Red de seguridad: reintento cada hora entre 06:00 y 14:00
EXEC msdb.dbo.sp_add_schedule
    @schedule_name = 'CEMSYS - Reintento horario',
    @freq_type = 4,
    @freq_interval = 1,
    @freq_subday_type = 8,      -- unidad: horas
    @freq_subday_interval = 1,  -- cada 1 hora
    @active_start_time = 060000,
    @active_end_time = 140000;
GO
 
EXEC msdb.dbo.sp_attach_schedule
    @job_name = 'CEMSYS - Actualizar Estado Concesiones',
    @schedule_name = 'CEMSYS - Reintento horario';
GO
 
EXEC msdb.dbo.sp_add_jobserver
    @job_name = 'CEMSYS - Actualizar Estado Concesiones',
    @server_name = '(LOCAL)';
GO
 
/* ======================================================================
   BOTÓN MANUAL DENTRO DEL SISTEMA
   Desde el backend, ejecutar con @Forzar = 1:
 
       EXEC dbo.sp_ActualizarEstadoConcesiones @Forzar = 1;
 
   Es seguro llamarlo aunque ya haya corrido el job automático ese día:
   el WHERE del procedimiento solo toca trámites que TODAVÍA están en
   Vigente, así que no puede duplicar cambios ni historial.
 
   NOTA - SQL Server Express: si tu instancia es Express, no tiene SQL
   Server Agent disponible y la sección 3 de este script va a fallar.
   En ese caso, usar el Programador de tareas de Windows para llamar:
       sqlcmd -S localhost -d cemsys -Q "EXEC dbo.sp_ActualizarEstadoConcesiones"
   con la casilla "Ejecutar tarea lo antes posible después de una hora
   de inicio perdida" tildada, para lograr el mismo efecto de
   recuperación ante cortes de luz.
   ====================================================================== */