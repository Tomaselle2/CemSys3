using CemSys3.Business.TramiteConcesion;
using CemSys3.Enumerables;
using CemSys3.Interfaces.PlantillaTramite;

namespace CemSys3.Business.PlantillaTramite
{
    public class StrategyFactory : IStrategyFactory
    {
        private readonly IServiceProvider _provider;

        public StrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IGeneradorAutorizacionesStrategy GetStrategy(int tipoTramiteId)
        {
            return tipoTramiteId switch
            {
                (int)TipoTramiteEnum.CambioTitular =>
                    _provider.GetRequiredService<CambioTitularStrategy>(),

                _ => throw new Exception("Strategy no implementada")
            };
        }
    }
}
