using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;

namespace CemSys3.DTOs.Parcela
{
    public class ParcelaHistorialDTO
    {
        public int Id { get; set; }
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int TipoParcelaId { get; set; }
        public int? TipoNichoId { get; set; }
        public int? TipoPanteonId { get; set; }
        public string? NombrePanteon { get; set; }
        public string? infoAdicional { get; set; }
        public int CantidadDifuntosActuales { get; set; }

        public IEnumerable<DifuntoHistorialParcelaDTO> DifuntosActuales { get; set; } = new List<DifuntoHistorialParcelaDTO>();
        public IEnumerable<DifuntoHistorialParcelaDTO> DifuntosHistoricos { get; set; } = new List<DifuntoHistorialParcelaDTO>();
        public IEnumerable<TramiteDTO> Tramites { get; set; } = new List<TramiteDTO>();

    }
}
