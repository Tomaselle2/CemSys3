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

                (int)TipoTramiteEnum.Cremacion =>
                _provider.GetRequiredService<CremacionStrategy>(),

                (int)TipoTramiteEnum.Traslado =>
                _provider.GetRequiredService<TrasladoStrategy>(),

                (int)TipoTramiteEnum.Reduccion =>
               _provider.GetRequiredService<ReduccionStrategy>(),

                _ => throw new Exception("Strategy no implementada")
            };
        }

        public ITramiteCreateStrategy GetCreateStrategy(int tipoTramiteId)
        {
            return tipoTramiteId switch
            {
                // Solicitar la interfaz en lugar de la clase concreta para evitar el error de conversión.
                // Asegúrate de que en la configuración de DI cada estrategia esté registrada
                // también como ITramiteCreateStrategy si procede.
                (int)TipoTramiteEnum.CambioTitular =>
                    _provider.GetRequiredService<CambioTitularStrategy>(),

                (int)TipoTramiteEnum.AceptacionTitular =>
                    _provider.GetRequiredService<AceptacionTitularStrategy>(),

                (int)TipoTramiteEnum.Cremacion =>
                _provider.GetRequiredService<CremacionStrategy>(),

                (int)TipoTramiteEnum.Traslado =>
                _provider.GetRequiredService<TrasladoStrategy>(),

                (int)TipoTramiteEnum.Reduccion =>
                _provider.GetRequiredService<ReduccionStrategy>(),

                _ => throw new Exception("CreateStrategy no implementada")
            };
        }
    }
}
