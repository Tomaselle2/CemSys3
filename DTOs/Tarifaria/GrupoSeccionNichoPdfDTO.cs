namespace CemSys3.DTOs.Tarifaria
{
    public class GrupoSeccionNichoPdfDTO
    {
        // Nombres de las secciones agrupadas: "C, D, H, I, 14"
        public string NombreSecciones { get; set; } = string.Empty;

        // Filas agrupadas con sus precios por año de concesión
        public List<FilaNichoPdfDTO> Filas { get; set; } = new();
    }
}
