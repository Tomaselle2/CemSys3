using CemSys3.Business.TramiteConcesion;
using CemSys3.Enumerables;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.TramitesConcesion;

namespace CemSys3.Business.PlantillaTramite
{
    public class StrategyFactory : IStrategyFactory
    {
        private readonly IServiceProvider _provider;

        public StrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ITramiteStrategy GetStrategy(int tipoTramiteId)
        {
            return tipoTramiteId switch
            {
                (int)TipoTramiteEnum.CambioTitular =>
                    _provider.GetRequiredService<CambioTitularStrategy>(),

                (int)TipoTramiteEnum.AceptacionTitular =>
                    _provider.GetRequiredService<AceptacionTitularStrategy>(),

                _ => throw new Exception("Strategy no implementada")
            };
        }
    }
}
