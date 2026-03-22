using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.Concesion
{
    public class InfoGeneralDTO
    {
        public int TramiteId { get; set; }
        public int EstadoTramiteId { get; set; }
        public int ParcelaId { get; set; }
        public string? TipoParcela { get; set; }
        public int SeccionId { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public int? NroConcesion { get; set; }
        public DateOnly? Vencimiento { get; set; }
        public string InfoAdicional { get; set; } = string.Empty;
        public List<DifuntoContratoDTO> Difuntos { get; set; } = new List<DifuntoContratoDTO>();
        public List<TitularesContratoDTO> Titulares { get; set; } = new List<TitularesContratoDTO>();

    }
}
