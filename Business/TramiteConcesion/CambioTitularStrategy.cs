using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.Enumerables;
using CemSys3.Helpers;
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

                string difuntosFormateados = DifuntoFormatter.FormatearDifuntos(dto.Difuntos);

                var primerTitular = dto.TitularesActuales.FirstOrDefault();
                string sexoReferencia = primerTitular?.Sexo ?? "masculino"; // Valor por defecto

                foreach (var plantilla in plantillas)
                {
                    var variables = new Dictionary<string, string>
                {
                    { "Fecha", DateTime.Now.ToLongDateString() },
                    { "articuloTitularActual", sexoReferencia  == "masculino" ? "el" : "la"},
                    { "sr/sraTitularActual", sexoReferencia  == "masculino" ? "Sr." : "Sra."},
                    { "TitularesActuales", string.Join(", ", dto.TitularesActuales.Select(t => t.Apellido.ToUpper() + " " + t.Nombre.ToUpper())) },
                    { "DniTitularActual", string.Join(", ", dto.TitularesActuales.Select(t => StringExtensions.FormatearDni(t.Dni))) },
                    { "Parcela", ParcelaFormatter.ObtenerParcela(dto.TipoParcela, dto.NroParcela, dto.NroFila, dto.NombreSeccion.ToUpper()) },
                    { "Difuntos", difuntosFormateados },
                    { "articuloNuevoTitular", persona.Sexo == "masculino" ? "al" : "a la"},
                    { "sr/sraNuevoTitular", persona.Sexo == "masculino" ? "Sr." : "Sra."},
                    { "NuevosTitulares", nuevoTitular.Apellido.ToUpper() + " " + nuevoTitular.Nombre.ToUpper() },
                    { "DniNuevosTitulares", StringExtensions.FormatearDni(nuevoTitular.Dni)  },

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

    }
}
