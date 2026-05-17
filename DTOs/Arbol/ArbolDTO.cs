namespace CemSys3.DTOs.Arbol
{
    public class ArbolDTO
    {
        public int TramiteId { get; set; }
        public string JsonDiagrama { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

    }
}
