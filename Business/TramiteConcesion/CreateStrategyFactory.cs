using CemSys3.Enumerables;
using CemSys3.Interfaces.TramitesConcesion;

namespace CemSys3.Business.TramiteConcesion
{
    public class CreateStrategyFactory : ICreateStrategyFactory
    {
        private readonly IServiceProvider _provider;

        public CreateStrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public object GetCreateStrategy(int tipoTramiteId)
        {
            return tipoTramiteId switch
            {
                (int)TipoTramiteEnum.CambioTitular =>
                    _provider.GetRequiredService<CambioTitularStrategy>(),

                (int)TipoTramiteEnum.AceptacionTitular =>
                    _provider.GetRequiredService<AceptacionTitularStrategy>(),

                _ => throw new Exception("CreateStrategy no implementada")
            };
        }
    }
}
