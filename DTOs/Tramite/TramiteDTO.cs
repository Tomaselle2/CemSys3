namespace CemSys3.DTOs.Tramite
{
    public class TramiteDTO
    {
        public int Id { get; set; }

        public bool Visibilidad { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int TipoTramiteId { get; set; }

        public int UsuarioId { get; set; }

        public int EstadoActualId { get; set; }
    }
}
