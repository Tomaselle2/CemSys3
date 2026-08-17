using CemSys3.DTOs.Backup;

namespace CemSys3.Interfaces.Backup
{
    public interface IBackupService
    {
        /// <summary>
        /// Devuelve las ultimas ejecuciones del job de backup de CemSys,
        /// una fila por corrida (resumen del job completo, no paso por paso).
        /// </summary>
        Task<List<BackupEjecucionDto>> ObtenerUltimasEjecucionesAsync(int cantidad = 20);

        /// <summary>
        /// Dispara el job de backup de CemSys de forma manual (msdb.dbo.sp_start_job).
        /// La ejecucion es asincrona en el Agent: este metodo solo confirma
        /// que el job arranco, no que haya terminado.
        /// </summary>
        Task<(bool Exito, string Mensaje)> EjecutarBackupManualAsync();
    }
}
