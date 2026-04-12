namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IStrategyFactory
    {
        IGeneradorAutorizacionesStrategy GetStrategy(int tipoTramiteId);
    }
}
