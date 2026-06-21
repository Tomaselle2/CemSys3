using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.CargaDifunto
{
    public class CargaDifuntoDTO
    {
        public int TramiteId { get; set; }

        public bool Visibilidad { get; set; }
        public int UsuarioLogueadoId { get; set; }

        public int ParcelaId { get; set; }

        public int DifuntoId { get; set; }

        public int EstadoDifuntoId { get; set; }

        public string? InformacionAdicional { get; set; }
        public int? PersonaExistenteId { get; set; }

        public PersonaDTO Difunto { get; set; } = new PersonaDTO();
    }
}
