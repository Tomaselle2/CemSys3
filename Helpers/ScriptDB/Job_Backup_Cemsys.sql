/* ============================================================
   CemSys - Backup diario FULL + verificacion + limpieza
   ============================================================
   Este script:
     1) Crea dos stored procedures en msdb con la logica real
     2) Crea el Job automatico (con schedule diario 00:30) - respeta
        el chequeo de "ya existe backup hoy"
     3) Crea un segundo Job SIN schedule para disparo manual - fuerza
        un backup nuevo siempre, ignorando ese chequeo
     4) Crea un proc wrapper para disparo manual desde la app
 
   Ejecutar como usuario con permisos sysadmin.
   Ajustar @BackupPath y @owner_login_name segun corresponda.
   ============================================================ */
 
USE msdb;
GO

-------------------------------------------------------------
-- 1) Backup FULL + verificacion (solo si no se hizo ya hoy)
-------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_CemSys_BackupYVerificar
    @Forzar BIT = 0   -- 1 = ignorar el chequeo de "ya existe backup hoy" (uso manual)
AS
BEGIN
    SET NOCOUNT ON;
 
    DECLARE @BackupPath      NVARCHAR(500) = N'D:\SQLBackups\cemsys\';
    DECLARE @FileName        NVARCHAR(500);
    DECLARE @Date            NVARCHAR(8)  = CONVERT(NVARCHAR(8), GETDATE(), 112);
    DECLARE @Time            NVARCHAR(6)  = REPLACE(CONVERT(NVARCHAR(8), GETDATE(), 108), ':', '');
    DECLARE @YaHizoHoy       BIT = 0;
    DECLARE @UltimoBackup    NVARCHAR(500);
    DECLARE @ArchivoExiste   INT = 0;
    DECLARE @ErrMsg          NVARCHAR(4000);
 
    -- Nos aseguramos de que exista la carpeta destino (evita fallos silenciosos)
    EXEC master.dbo.xp_create_subdir @BackupPath;
 
    -- Buscamos el ultimo backup FULL de hoy segun el historial de msdb
    SELECT TOP 1 @UltimoBackup = bmf.physical_device_name
    FROM msdb.dbo.backupset bs
    JOIN msdb.dbo.backupmediafamily bmf ON bmf.media_set_id = bs.media_set_id
    WHERE bs.database_name = 'cemsys'
      AND bs.type = 'D'   -- D = Full
      AND bs.backup_finish_date >= CAST(GETDATE() AS DATE)
      AND bs.backup_finish_date <  DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
    ORDER BY bs.backup_finish_date DESC;
 
    -- No alcanza con que el historial diga que existe: el archivo pudo
    -- haber sido borrado despues (por ejemplo por el paso de limpieza).
    -- Confirmamos que siga fisicamente en disco.
    IF @UltimoBackup IS NOT NULL
    BEGIN
        SELECT @ArchivoExiste = file_exists
        FROM sys.dm_os_file_exists(@UltimoBackup);
    END
 
    IF @UltimoBackup IS NOT NULL AND @ArchivoExiste = 1 AND @Forzar = 0
    BEGIN
        SET @YaHizoHoy = 1;
        PRINT 'Ya existe un backup FULL de CemSys de hoy y el archivo esta presente (' + @UltimoBackup + '). Se omite el backup.';
    END
    ELSE IF @Forzar = 1
    BEGIN
        PRINT 'Backup forzado manualmente: se ignora el chequeo de backup existente.';
    END
 
    IF @YaHizoHoy = 0
    BEGIN
        SET @FileName = @BackupPath + N'cemsys_' + @Date + N'_' + @Time + N'.bak';
 
        BEGIN TRY
            PRINT 'Iniciando backup FULL de CemSys -> ' + @FileName;
 
            BACKUP DATABASE cemsys
            TO DISK = @FileName
            WITH
                COMPRESSION,
                CHECKSUM,
                INIT,
                STATS = 10;
 
            PRINT 'Backup finalizado. Verificando integridad...';
 
            RESTORE VERIFYONLY
            FROM DISK = @FileName
            WITH CHECKSUM;
 
            PRINT 'Verificacion OK.';
        END TRY
        BEGIN CATCH
            SET @ErrMsg = ERROR_MESSAGE();
            RAISERROR('Error en backup o verificacion de CemSys: %s', 16, 1, @ErrMsg);
        END CATCH
    END
END
GO


-------------------------------------------------------------
-- 2) Eliminar backups antiguos (deja solo los ultimos 15)
-------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_CemSys_EliminarBackupsAntiguos
AS
BEGIN
    SET NOCOUNT ON;
 
    DECLARE @BackupPath NVARCHAR(500) = N'D:\SQLBackups\cemsys\';
    DECLARE @Files TABLE
    (
        FileName NVARCHAR(500),
        FileDate DATETIME
    );
    DECLARE @FileName NVARCHAR(500);
    DECLARE @Borrados INT = 0;
    DECLARE @ErrMsg   NVARCHAR(4000);
 
    BEGIN TRY
        INSERT INTO @Files (FileName, FileDate)
        SELECT full_filesystem_path, creation_time
        FROM sys.dm_os_enumerate_filesystem(@BackupPath, '*.bak')
        WHERE is_directory = 0;
 
        -- Sacamos de @Files los 15 mas recientes: lo que queda en @Files
        -- despues de este DELETE es justamente lo viejo, que hay que borrar.
        DELETE FROM @Files
        WHERE FileName IN (
            SELECT TOP 15 FileName
            FROM @Files
            ORDER BY FileDate DESC
        );
 
        DECLARE FileCursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT FileName FROM @Files;
 
        OPEN FileCursor;
        FETCH NEXT FROM FileCursor INTO @FileName;
 
        WHILE @@FETCH_STATUS = 0
        BEGIN
            PRINT 'Eliminando backup antiguo: ' + @FileName;
            EXEC master.dbo.xp_delete_file 0, @FileName;
            SET @Borrados = @Borrados + 1;
            FETCH NEXT FROM FileCursor INTO @FileName;
        END;
 
        CLOSE FileCursor;
        DEALLOCATE FileCursor;
 
        PRINT 'Limpieza finalizada. Archivos eliminados: ' + CAST(@Borrados AS NVARCHAR(10));
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'FileCursor') >= -1
        BEGIN
            CLOSE FileCursor;
            DEALLOCATE FileCursor;
        END
        SET @ErrMsg = ERROR_MESSAGE();
        RAISERROR('Error al eliminar backups antiguos de CemSys: %s', 16, 1, @ErrMsg);
    END CATCH
END
GO
 

-------------------------------------------------------------
-- 3) Crear el Job de SQL Server Agent
-------------------------------------------------------------
DECLARE @JobId   BINARY(16);
DECLARE @JobName sysname = N'CemSys - Backup diario y limpieza';
 
-- Si ya existe (por reejecutar este script), lo borramos primero
IF EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = @JobName)
    EXEC msdb.dbo.sp_delete_job @job_name = @JobName;
 
EXEC msdb.dbo.sp_add_job
    @job_name         = @JobName,
    @enabled          = 1,
    @description      = N'Backup FULL diario de CemSys, verificacion de integridad y limpieza de backups antiguos (conserva los ultimos 15).',
    @owner_login_name = N'sa',   -- ajustar si "sa" no existe o no corresponde
    @job_id           = @JobId OUTPUT;
 
-- Paso 1: Backup y verificar
EXEC msdb.dbo.sp_add_jobstep
    @job_id             = @JobId,
    @step_id            = 1,
    @step_name          = N'1 - Backup y Verificar CemSys',
    @subsystem          = N'TSQL',
    @database_name      = N'msdb',
    @command            = N'EXEC dbo.usp_CemSys_BackupYVerificar;',
    @on_success_action  = 3,   -- ir al paso siguiente
    @on_fail_action     = 2,   -- salir del job informando error
    @retry_attempts     = 1,
    @retry_interval     = 5;
 
-- Paso 2: Eliminar backups antiguos
EXEC msdb.dbo.sp_add_jobstep
    @job_id             = @JobId,
    @step_id            = 2,
    @step_name          = N'2 - Eliminar backups antiguos',
    @subsystem          = N'TSQL',
    @database_name      = N'msdb',
    @command            = N'EXEC dbo.usp_CemSys_EliminarBackupsAntiguos;',
    @on_success_action  = 1,   -- salir informando exito
    @on_fail_action     = 2,   -- salir informando error
    @retry_attempts     = 1,
    @retry_interval     = 5;
 
EXEC msdb.dbo.sp_update_job
    @job_id        = @JobId,
    @start_step_id = 1;
 
-- Schedule diario a las 00:30
EXEC msdb.dbo.sp_add_schedule
    @schedule_name      = N'Diario 00:30 - CemSys',
    @freq_type          = 4,        -- diario
    @freq_interval      = 1,
    @active_start_time  = 003000,   -- 00:30:00
    @enabled            = 1;
 
EXEC msdb.dbo.sp_attach_schedule
    @job_id        = @JobId,
    @schedule_name = N'Diario 00:30 - CemSys';
 
EXEC msdb.dbo.sp_add_jobserver
    @job_id      = @JobId,
    @server_name = N'(local)';
GO
 
-------------------------------------------------------------
-- 4) Segundo Job: disparo manual FORZADO (sin schedule)
--    Se dispara solo desde la app via sp_start_job. Siempre hace
--    un backup nuevo, ignorando el chequeo de "ya existe backup hoy".
--    Queda en el mismo historial de sysjobhistory que el job automatico.
-------------------------------------------------------------
DECLARE @JobIdManual   BINARY(16);
DECLARE @JobNameManual sysname = N'CemSys - Backup Manual';
 
IF EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = @JobNameManual)
    EXEC msdb.dbo.sp_delete_job @job_name = @JobNameManual;
 
EXEC msdb.dbo.sp_add_job
    @job_name         = @JobNameManual,
    @enabled          = 1,
    @description      = N'Backup FULL forzado de CemSys, disparado manualmente desde la app (ignora el chequeo de backup del dia).',
    @owner_login_name = N'sa',   -- ajustar si "sa" no existe o no corresponde
    @job_id           = @JobIdManual OUTPUT;
 
EXEC msdb.dbo.sp_add_jobstep
    @job_id             = @JobIdManual,
    @step_id            = 1,
    @step_name          = N'1 - Backup y Verificar CemSys (forzado)',
    @subsystem          = N'TSQL',
    @database_name      = N'msdb',
    @command            = N'EXEC dbo.usp_CemSys_BackupYVerificar @Forzar = 1;',
    @on_success_action  = 3,
    @on_fail_action     = 2,
    @retry_attempts     = 1,
    @retry_interval     = 5;
 
EXEC msdb.dbo.sp_add_jobstep
    @job_id             = @JobIdManual,
    @step_id            = 2,
    @step_name          = N'2 - Eliminar backups antiguos',
    @subsystem          = N'TSQL',
    @database_name      = N'msdb',
    @command            = N'EXEC dbo.usp_CemSys_EliminarBackupsAntiguos;',
    @on_success_action  = 1,
    @on_fail_action     = 2,
    @retry_attempts     = 1,
    @retry_interval     = 5;
 
EXEC msdb.dbo.sp_update_job
    @job_id        = @JobIdManual,
    @start_step_id = 1;
 
-- OJO: a proposito NO se le pega ningun schedule ni sp_add_jobserver
-- de servidor con schedule; igual necesita sp_add_jobserver para poder
-- correr en esta instancia (sin eso sp_start_job no lo encuentra disponible).
EXEC msdb.dbo.sp_add_jobserver
    @job_id      = @JobIdManual,
    @server_name = N'(local)';
GO

-------------------------------------------------------------
-- 4) Segundo Job: disparo manual FORZADO (sin schedule)
--    Se dispara solo desde la app via sp_start_job. Siempre hace
--    un backup nuevo, ignorando el chequeo de "ya existe backup hoy".
--    Queda en el mismo historial de sysjobhistory que el job automatico.
-------------------------------------------------------------
DECLARE @JobIdManual   BINARY(16);
DECLARE @JobNameManual sysname = N'CemSys - Backup Manual';
 
IF EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = @JobNameManual)
    EXEC msdb.dbo.sp_delete_job @job_name = @JobNameManual;
 
EXEC msdb.dbo.sp_add_job
    @job_name         = @JobNameManual,
    @enabled          = 1,
    @description      = N'Backup FULL forzado de CemSys, disparado manualmente desde la app (ignora el chequeo de backup del dia).',
    @owner_login_name = N'sa',   -- ajustar si "sa" no existe o no corresponde
    @job_id           = @JobIdManual OUTPUT;
 
EXEC msdb.dbo.sp_add_jobstep
    @job_id             = @JobIdManual,
    @step_id            = 1,
    @step_name          = N'1 - Backup y Verificar CemSys (forzado)',
    @subsystem          = N'TSQL',
    @database_name      = N'msdb',
    @command            = N'EXEC dbo.usp_CemSys_BackupYVerificar @Forzar = 1;',
    @on_success_action  = 3,
    @on_fail_action     = 2,
    @retry_attempts     = 1,
    @retry_interval     = 5;
 
EXEC msdb.dbo.sp_add_jobstep
    @job_id             = @JobIdManual,
    @step_id            = 2,
    @step_name          = N'2 - Eliminar backups antiguos',
    @subsystem          = N'TSQL',
    @database_name      = N'msdb',
    @command            = N'EXEC dbo.usp_CemSys_EliminarBackupsAntiguos;',
    @on_success_action  = 1,
    @on_fail_action     = 2,
    @retry_attempts     = 1,
    @retry_interval     = 5;
 
EXEC msdb.dbo.sp_update_job
    @job_id        = @JobIdManual,
    @start_step_id = 1;
 
-- OJO: a proposito NO se le pega ningun schedule ni sp_add_jobserver
-- de servidor con schedule; igual necesita sp_add_jobserver para poder
-- correr en esta instancia (sin eso sp_start_job no lo encuentra disponible).
EXEC msdb.dbo.sp_add_jobserver
    @job_id      = @JobIdManual,
    @server_name = N'(local)';
GO

USE msdb;
GO

ALTER PROCEDURE dbo.usp_CemSys_EjecutarBackupManual
WITH EXECUTE AS OWNER
AS
BEGIN
    SET NOCOUNT ON;
    EXEC msdb.dbo.sp_start_job @job_name = N'CemSys - Backup Manual';
END
GO
 
 






-----------------------------------------------------------------------


-- Reemplazá 'MSITUF\Tomi' por lo que te devolvio whoami
USE master;
GO
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'msituf\tomas')
    CREATE LOGIN [msituf\tomas] FROM WINDOWS;
GO

USE msdb;
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'msituf\tomas')
    CREATE USER [msituf\tomas] FOR LOGIN [msituf\tomas];
GO

ALTER ROLE SQLAgentReaderRole ADD MEMBER [msituf\tomas];
GO

GRANT EXECUTE ON dbo.usp_CemSys_EjecutarBackupManual TO [msituf\tomas];
GO

EXEC msdb.dbo.usp_CemSys_BackupYVerificar;
GO
EXEC msdb.dbo.usp_CemSys_EliminarBackupsAntiguos;