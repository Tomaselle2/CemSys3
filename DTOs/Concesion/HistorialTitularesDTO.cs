namespace CemSys3.DTOs.Concesion
{
    public class HistorialTitularesDTO
    {
        public int? ConcesionId { get; set; }
        public int? PersonaId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

    }
}
