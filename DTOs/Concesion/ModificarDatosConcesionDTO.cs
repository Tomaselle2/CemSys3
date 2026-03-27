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

    }
}
