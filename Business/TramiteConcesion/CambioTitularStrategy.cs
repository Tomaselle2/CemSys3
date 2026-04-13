using CemSys3.Enumerables;
using CemSys3.Interfaces.PlantillaTramite;

namespace CemSys3.Business.TramiteConcesion
{
    public class CambioTitularStrategy : IGeneradorAutorizacionesStrategy
    {
        private readonly IPlantillaTramite _plantillaService;
        private readonly IDocumentoTramiteService _documentoService;

        public CambioTitularStrategy(
            IPlantillaTramite plantillaService,
            IDocumentoTramiteService documentoService)
        {
            _plantillaService = plantillaService;
            _documentoService = documentoService;
        }

        public async Task GenerarAsync(int tramiteId, List<int> personasIds, int usuarioId, string parentesco)
        {
            var plantillas = await _plantillaService
                .ObtenerPorTipoTramiteAsync((int)TipoTramiteEnum.CambioTitular);

            foreach (var personaId in personasIds)
            {
                foreach (var plantilla in plantillas)
                {
                    var variables = new Dictionary<string, string>
                {
                    { "Fecha", DateTime.Now.ToShortDateString() },
                    { "PersonaId", personaId.ToString() }
                };

                    await _documentoService.CrearDesdePlantillaAsync(
                        plantilla.PlantillaId,
                        tramiteId,
                        usuarioId,
                        personaId,
                        parentesco,
                        variables
                    );
                }
            }
        }
    }
}
