using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.Concesion
{
    public class ConcesionDTO
    {
        public int TramiteId { get; set; }

        public int? Concesion { get; set; }

        public decimal? Precio { get; set; }

        public bool? Visibilidad { get; set; }

        public string? TipoParcela { get; set; }

        public DateOnly? Vencimiento { get; set; }

        public int ParcelaId { get; set; }

        public int? CantidadAniosId { get; set; }

        public int? CuotaId { get; set; }

        public int? UsuarioId { get; set; }

        public string? InformacionAdicional { get; set; }

        //---
        public int EstadoTramiteId { get; set; }
        public List<PersonaDTO>? Titulares { get; set; }
        public List<DifuntoContratoDTO> Difuntos { get; set; } = new List<DifuntoContratoDTO>();

        public string MensajeParcela { get; set; } = string.Empty;
    }
}
