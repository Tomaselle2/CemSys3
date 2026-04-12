namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IGeneradorAutorizacionesStrategy
    {
        Task GenerarAsync(int tramiteId, List<int> personasIds, int usuarioId);
    }
}
