namespace CemSys3.DTOs.PlantillaTramite
{
    public class PlantillaTramiteDTO
    {
        public int PlantillaId { get; set; }
        public int TramiteId { get; set; }

        public int TipoTramiteId { get; set; }

        public string? Nombre { get; set; }

        public string? Contenido { get; set; }
        public string Parentesco { get; set; } = string.Empty;
        public int PersonaId { get; set; }
        public bool? Activo { get; set; }
        public int TipoAutorizacionId { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
