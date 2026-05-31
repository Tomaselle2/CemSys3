using CemSys3.Interfaces.TramitesConcesion;

namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IStrategyFactory
    {
        ITramiteStrategy GetStrategy(int tipoTramiteId);
        ITramiteCreateStrategy GetCreateStrategy(int tipoTramiteId);
    }
}
