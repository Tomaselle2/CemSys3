namespace CemSys3.DTOs.Reportes
{
    public class ReportePersonaDniDuplicadoDTO
    {
        public string Dni { get; set; } = string.Empty;
        public int PersonaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int? CategoriaPersonaId { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public DateOnly? FechaDefuncion { get; set; }
        public string? Correo { get; set; }
        public string? Celular { get; set; }
    }
}
