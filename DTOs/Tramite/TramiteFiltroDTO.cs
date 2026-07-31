using System.ComponentModel.DataAnnotations;

namespace CemSys3.DTOs.Tramite
{
    public class TramiteFiltroDTO
    {
        public int? TramiteId { get; set; }
        public int? TipoTramiteId { get; set; }
        public int? EstadoActualId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaDesde { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaHasta { get; set; }

        public bool TieneFiltros =>
        TramiteId.HasValue || TipoTramiteId.HasValue || EstadoActualId.HasValue ||
        FechaDesde.HasValue || FechaHasta.HasValue;
    }
}
