namespace CemSys3.DTOs.Paginacion
{
    public class PaginadoResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();
    }
}
