using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.Concesion
{
    public class ModificarDatosConcesionDTO
    {
        public int TramiteId { get; set; }
        public int EstadoTramiteId { get; set; }
        public int? NroConcesion { get; set; }
        public DateOnly? Vencimiento { get; set; }
        public List<TitularesContratoDTO> Titulares { get; set; } = new List<TitularesContratoDTO>();
        public List<PersonaDTO>? TitularesPost { get; set; }


        private DateTime? _fechaInicio;

        public DateTime? FechaInicio
        {
            get => _fechaInicio;
            set => _fechaInicio = value;
        }

        // Propiedad auxiliar para el input datetime-local
        public string FechaInicioInput
        {
            get => _fechaInicio?.ToString("yyyy-MM-ddTHH:mm");
            set
            {
                if (DateTime.TryParse(value, out var fecha))
                    _fechaInicio = fecha;
            }
        }

        // Propiedad para mostrar en formato deseado
        public string FechaInicioDisplay => _fechaInicio?.ToString("dd/MM/yyyy HH:mm");

    }
}
