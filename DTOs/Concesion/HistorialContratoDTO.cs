namespace CemSys3.DTOs.Concesion
{
    public class HistorialContratoDTO
    {
        public int TramiteId { get; set; }
        public int? Concesion { get; set; }
        public int ParcelaId { get; set; }
        public DateTime FechaContrato { get; set; } = DateTime.Now;
        public bool EsRenovacion { get; set; }
        public int? UsuarioId { get; set; }
        public List<int> DifuntosIds { get; set; } = new List<int>();
    }
}
