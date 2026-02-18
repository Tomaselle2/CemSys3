namespace CemSys3.DTOs.Persona
{
    public class DifuntoHistorialParcelaDTO
    {
        public int Id { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaRetiro { get; set; }
        public string? Dni { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int? EstadoDifunto { get; set; }
    }
}
