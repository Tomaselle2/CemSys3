namespace CemSys3.DTOs.Reportes
{
    public class ReporteConcesionSinDifuntoDTO
    {
        public int? Concesion { get; set; }
        public string Seccion { get; set; } = string.Empty;
        public int? NroFila { get; set; }
        public int? NroParcela { get; set; }
        public DateOnly? Vencimiento { get; set; }
    }
}
