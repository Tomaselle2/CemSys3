using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.PlantillaTramite;

namespace CemSys3.Business.TramiteConcesion
{
    public class CambioTitularStrategy : IGeneradorAutorizacionesStrategy
    {
        private readonly IPlantillaTramite _plantillaService;
        private readonly IDocumentoTramiteService _documentoService;
        private readonly IPersona _personaService;

        public CambioTitularStrategy(
            IPlantillaTramite plantillaService,
            IDocumentoTramiteService documentoService,
            IPersona personaService)
        {
            _plantillaService = plantillaService;
            _documentoService = documentoService;
            _personaService = personaService;
        }

        public async Task GenerarAsync(GeneraStrategyDTO dto)
        {
            var plantillas = await _plantillaService
                .ObtenerPorTipoTramiteAsync((int)TipoTramiteEnum.CambioTitular);

            foreach (var nuevoTitular in dto.NuevosTitulares)
            {
                PersonaDTO persona = await _personaService.Get(nuevoTitular.Id.Value);

                persona.Nombre = nuevoTitular.Nombre;
                persona.Apellido = nuevoTitular.Apellido;
                persona.Domicilio = nuevoTitular.Domicilio;
                persona.Celular = nuevoTitular.Celular;
                persona.Correo = nuevoTitular.CorreoElectronico;
                
                int personaId = await _personaService.Update(persona);

                foreach (var plantilla in plantillas)
                {
                    var variables = new Dictionary<string, string>
                {
                    { "Fecha", DateTime.Now.ToShortDateString() },
                    { "NuevosTitulares", nuevoTitular.Apellido.ToUpper() + " " + nuevoTitular.Nombre.ToUpper() },
                    { "TitularesActuales", string.Join(", ", dto.TitularesActuales.Select(t => t.Apellido.ToUpper() + " " + t.Nombre.ToUpper())) },
                    { "Parcela", ObtenerParcela(dto.TipoParcela, dto.NroParcela, dto.NroFila, dto.NombreSeccion.ToUpper()) }
                };

                    await _documentoService.CrearDesdePlantillaAsync(
                        plantilla.PlantillaId,
                        dto.TramiteId,
                        dto.UsuarioId,
                        nuevoTitular.Id,
                        dto.Parentesco,
                        variables
                    );
                }
            }
        }

        private string ObtenerParcela(string TipoParcela, int NroParcela, int NroFila, string NombreSeccion)
        {
            if (TipoParcela == "Nicho")
                return $"Nicho {NroParcela} Secc {NombreSeccion} Fila {  NroFila}";

            if (TipoParcela == "Fosa")
                return $"Fosa {NroParcela} Secc {NombreSeccion}";

            if (TipoParcela == "Panteón")
                return $"Lote {NroParcela} Secc {NombreSeccion} (panteón)";

            return "";
        }
    }
}
