namespace CemSys3.DTOs.Reportes
{
    public class ReporteTitularEsDifuntoDTO
    {
        public int? Concesion { get; set; }
        public string Seccion { get; set; } = string.Empty;
        public int? NroFila { get; set; }
        public int? NroParcela { get; set; }
        public int PersonaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateOnly? Vencimiento { get; set; }
    }
}
