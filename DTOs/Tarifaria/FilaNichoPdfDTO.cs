namespace CemSys3.DTOs.Tarifaria
{
    public class FilaNichoPdfDTO
    {
        // Etiqueta de la fila: "1° FILA (ABAJO)", "2° y 3° FILA", etc.
        public string Etiqueta { get; set; } = string.Empty;

        // Precio por años de concesión (clave = años: 5, 10, 15, 25)
        public Dictionary<int, decimal> PreciosPorAnio { get; set; } = new();
    }
}
