use cemsys
go

--solo cuando se termines de crear las fosas
--antes de carga inicial de fallecidos, agrega la fosa 0 en seccion antig
insert into Parcelas(visibilidad, nroParcela, nroFila, cantidadDifuntos, nombrePanteon, informacionAdicional, seccionId, tipoParcelaId) 
values 
(1, 0, 1, 0, '', '', 66, 2);

update Secciones set nroParcelas = 35 where id = 66

--// para saber cuantas concesiones no tiene difuntos

SELECT
    c.Concesion,
    s.Nombre AS Seccion,
    p.NroFila,
    p.NroParcela,
    c.Vencimiento
FROM Concesiones c
INNER JOIN Parcelas p
    ON c.ParcelaId = p.Id
INNER JOIN Secciones s
    ON p.SeccionId = s.Id
LEFT JOIN ParcelaDifuntos pd
    ON pd.ParcelaId = p.Id
    AND pd.FechaRetiro IS NULL
WHERE pd.Id IS NULL
  AND c.FechaFin IS NULL      -- No está caducada
ORDER BY c.Concesion;

---------------------------------------------------

--// para hacer backup manualmente
BACKUP DATABASE cemsys
TO DISK = 'D:\Backups\cemsys_Backup.bak'
WITH
    FORMAT,
    INIT,
    COMPRESSION,
    STATS = 10;

--// para saber donde estan los archivos de filestream
RESTORE FILELISTONLY
FROM DISK = 'D:\Backups\cemsys_Backup.bak';

--// para restaurar un backup

RESTORE DATABASE cemsys
FROM DISK = 'D:\Backups\cemsys_Backup.bak'
WITH
    RECOVERY,
    STATS = 10;

--restaura la base de datos en otra carpeta el filestream
RESTORE DATABASE CemSys
FROM DISK = 'D:\Backups\cemsys_Backup.bak'
WITH
    MOVE 'CemSys'
        TO 'C:\SQLData\cemsys.mdf',

    MOVE 'CemSys_log'
        TO 'C:\SQLData\cemsys_log.ldf',

    MOVE 'CemSysFileStream'
        TO 'C:\CemsysArchive3',

    RECOVERY,
    STATS = 10;