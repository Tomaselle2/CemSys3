namespace CemSys3.DTOs.PlantillaTramite
{
    public class PlantillaTramiteDTO
    {
        public int Id { get; set; }

        public int TipoTramiteId { get; set; }

        public string? Nombre { get; set; }

        public string? Contenido { get; set; }

        public int? TipoEscenario { get; set; }

        public bool? Activo { get; set; }
        public int Codigo { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
