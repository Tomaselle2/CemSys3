namespace CemSys3.DTOs.PlantillaTramite
{
    public class PlantillaTramiteDTO
    {
        public int PlantillaId { get; set; }
        public int TipoTramiteId { get; set; }
        public int TipoAutorizacionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public bool? Activo { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
