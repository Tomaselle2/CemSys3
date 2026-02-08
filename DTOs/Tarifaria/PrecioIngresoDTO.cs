namespace CemSys3.DTOs.Tarifaria
{
    public class PrecioIngresoDTO
    {
        public string NombreRegla { get; set; } = null!;
        public int TipoParcelaId { get; set; }
        public List<TemaIngresoDTO> Temas { get; set; } = new List<TemaIngresoDTO>();
    }
}
