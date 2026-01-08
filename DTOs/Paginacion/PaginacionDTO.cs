namespace CemSys3.DTOs.Paginacion
{
    public class PaginacionDTO
    {
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public string? Accion { get; set; }
        public string? Controlador { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalRegistros { get; set; }


        // Para mantener filtros (opcional)
        public Dictionary<string, string>? Parametros { get; set; } = new Dictionary<string, string>();
    }
}
