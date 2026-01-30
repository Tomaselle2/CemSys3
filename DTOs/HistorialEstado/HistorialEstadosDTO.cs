namespace CemSys3.DTOs.HistorialEstado
{
    public class HistorialEstadosDTO
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int TramiteId { get; set; }

        public int EstadoTramiteId { get; set; }
    }
}
