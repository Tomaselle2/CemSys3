using CemSys3.Interfaces.Concesion;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CemSys3.Business.Concesion
{
    public class ActualizarConcesionesAutomaticas : IActualizarConcesionesAutomaticas
    {
        private readonly string _connectionString;

        public ActualizarConcesionesAutomaticas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion")
                ?? throw new InvalidOperationException("Falta la connection string 'Conexion'.");
        }

        public async Task<int> ActualizarEstadoConcesionesAsync(bool forzar = false, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand("dbo.sp_ActualizarEstadoConcesiones", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            command.Parameters.Add(new SqlParameter("@Forzar", SqlDbType.Bit) { Value = forzar });

            var cantidadParam = new SqlParameter("@CantidadActualizadas", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(cantidadParam);

            await connection.OpenAsync(cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);

            return (int)(cantidadParam.Value == DBNull.Value ? 0 : cantidadParam.Value);
        }
    }
}
