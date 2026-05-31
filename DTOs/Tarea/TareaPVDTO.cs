namespace CemSys3.DTOs.Tarea
{
    public class TareaPVDTO
    {
        public List<TareaDTO> Tareas { get; set; } = new List<TareaDTO>();
        public int TramiteId { get; set; }
        public int TipoTramiteId { get; set; }
        public int EstadoTramiteId { get; set; }
        public int ConcesionId { get; set; }
        public string returnUrl = string.Empty;

    }
}
