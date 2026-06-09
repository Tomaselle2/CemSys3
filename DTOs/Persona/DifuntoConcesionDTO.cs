namespace CemSys3.DTOs.Persona
{
    public class DifuntoConcesionDTO
    {
        public int ParcelaDifuntoId { get; set; }
        public int DifuntoId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaRetiro { get; set; }
        public int? TramiteIngresoId { get; set; }
        public int? TramiteRetiroId { get; set; }
    }
}
