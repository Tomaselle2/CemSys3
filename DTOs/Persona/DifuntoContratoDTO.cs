namespace CemSys3.DTOs.Persona
{
    public class DifuntoContratoDTO
    {
        public int Id { get; set; }
        public string? DNI { get; set; }
        public string? Nombre { get; set; }
        public string?Apellido { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int? EstadoDifuntoId { get; set; }
    }
}
