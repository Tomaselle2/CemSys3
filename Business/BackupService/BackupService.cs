using CemSys3.DTOs.Backup;
using CemSys3.Interfaces.Backup;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CemSys3.Business.Backup
{
    public class BackupService : IBackupService
    {
        private readonly string _connectionString;
        private const string JobNameAutomatico = "CemSys - Backup diario y limpieza";
        private const string JobNameManual = "CemSys - Backup Manual";

        public BackupService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServerAgent")
                ?? throw new InvalidOperationException(
                    "No se encontro la connection string 'SqlServerAgent' en appsettings.json.");
        }

        public async Task<List<BackupEjecucionDto>> ObtenerUltimasEjecucionesAsync(int cantidad = 20)
        {
            const string sql = @"
            SELECT TOP (@Cantidad)
                j.name AS JobName,
                CASE WHEN j.name = @JobNameManual THEN N'Manual' ELSE N'Automático' END AS Origen,
                msdb.dbo.agent_datetime(h.run_date, h.run_time) AS FechaEjecucion,
                h.run_status AS RunStatus,
                STUFF(STUFF(RIGHT('000000' + CAST(h.run_duration AS VARCHAR(6)), 6), 5, 0, ':'), 3, 0, ':') AS Duracion,
                h.message AS Mensaje
            FROM msdb.dbo.sysjobhistory h
            JOIN msdb.dbo.sysjobs j ON j.job_id = h.job_id
            WHERE j.name IN (@JobNameAutomatico, @JobNameManual)
              AND h.step_id = 0   -- resumen del job completo, no cada paso individual
            ORDER BY h.run_date DESC, h.run_time DESC;";

            var resultado = new List<BackupEjecucionDto>();

            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Cantidad", cantidad);
            cmd.Parameters.AddWithValue("@JobNameAutomatico", JobNameAutomatico);
            cmd.Parameters.AddWithValue("@JobNameManual", JobNameManual);

            await conn.OpenAsync();
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                resultado.Add(new BackupEjecucionDto
                {
                    JobName = reader.GetString(reader.GetOrdinal("JobName")),
                    Origen = reader.GetString(reader.GetOrdinal("Origen")),
                    FechaEjecucion = reader.GetDateTime(reader.GetOrdinal("FechaEjecucion")),
                    RunStatus = reader.GetInt32(reader.GetOrdinal("RunStatus")),
                    Duracion = reader.GetString(reader.GetOrdinal("Duracion")),
                    Mensaje = reader.IsDBNull(reader.GetOrdinal("Mensaje"))
                        ? string.Empty
                        : reader.GetString(reader.GetOrdinal("Mensaje"))
                });
            }

            return resultado;
        }

        public async Task<(bool Exito, string Mensaje)> EjecutarBackupManualAsync()
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await using var cmd = new SqlCommand("msdb.dbo.usp_CemSys_EjecutarBackupManual", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return (true,
                    "El backup manual se disparo correctamente y va a crear un archivo nuevo " +
                    "El Agent lo ejecuta en segundo " +
                    "plano: actualiza la pagina en unos segundos para ver el resultado.");
            }
            catch (SqlException ex)
            {
                return (false, $"No se pudo iniciar el backup: {ex.Message}");
            }
        }
    }
}
