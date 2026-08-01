namespace CemSys3.Interfaces.Concesion
{
    public interface IActualizarConcesionesAutomaticas
    {
        Task<int> ActualizarEstadoConcesionesAsync(bool forzar = false, CancellationToken cancellationToken = default);

    }
}
