namespace CemSys3.DTOs.Tarifaria
{
    public class PrecioFosaPdfDTO
    {
        // "Por 15 años" / "Por 25 años"
        public string Etiqueta { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }
}
