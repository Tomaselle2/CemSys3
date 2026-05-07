using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.Persona;
using Org.BouncyCastle.Asn1.Crmf;

namespace CemSys3.DTOs.TramitesConcesion.Cremacion
{
    public class CremacionDTO
    {
        public int TramiteId { get; set; }
        public int EstadoTramiteId { get; set; }
        public int TipoTramiteId { get; set; }
        public int ParcelaId { get; set; }
        public string? TipoParcela { get; set; }
        public string? NombreSeccion { get; set; }

        public int CementerioId { get; set; }
        public int? NroParcela { get; set; }
        public int? NroFila { get; set; }
        public int? NroConcesion { get; set; }
        public int ConcesionId { get; set; }

        public List<TitularesContratoDTO> TitularesActuales { get; set; } = new();

        public List<DifuntoContratoDTO> Difuntos { get; set; } = new List<DifuntoContratoDTO>();

        public List<CementerioRequestDTO> Cementerios { get; set; } = new List<CementerioRequestDTO>();


        public string? InfoAdicional { get; set; }
    }
}
