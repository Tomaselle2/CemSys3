using CemSys3.DTOs.Persona;
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

        public async Task GenerarAsync(int tramiteId, List<TitularesContratoDTO> titularesActuales, List<TitularesContratoDTO> nuevosTitulares, int usuarioId, string parentesco)
        {
            var plantillas = await _plantillaService
                .ObtenerPorTipoTramiteAsync((int)TipoTramiteEnum.CambioTitular);

            foreach (var nuevoTitular in nuevosTitulares)
            {
                foreach (var plantilla in plantillas)
                {
                    var variables = new Dictionary<string, string>
                {
                    { "Fecha", DateTime.Now.ToShortDateString() },
                    { "NuevosTitulares", nuevoTitular.Apellido.ToUpper() + nuevoTitular.Nombre.ToUpper() },
                    { "TitularesActuales", string.Join(", ", titularesActuales.Select(t => t.Apellido.ToUpper() + t.Nombre.ToUpper())) }
                };

                    await _documentoService.CrearDesdePlantillaAsync(
                        plantilla.PlantillaId,
                        tramiteId,
                        usuarioId,
                        nuevoTitular.Id,
                        parentesco,
                        variables
                    );
                }
            }
        }
    }
}
