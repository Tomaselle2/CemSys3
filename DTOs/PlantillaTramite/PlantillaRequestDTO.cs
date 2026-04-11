namespace CemSys3.DTOs.PlantillaTramite
{
    public class PlantillaRequestDTO
    {
        public int PlantillaId { get; set; }
        public string TipoTramite { get; set; } = string.Empty;
        public object Data { get; set; } = new();
    }
}
