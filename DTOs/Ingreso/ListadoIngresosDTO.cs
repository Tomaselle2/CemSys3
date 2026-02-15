namespace CemSys3.DTOs.Ingreso
{
    public class ListadoIngresosDTO
    {
        public int TramiteId { get; set; }
        public int EstadoTramiteId { get; set; }
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public int TipoParcelaId { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public string NombreDifunto { get; set; } = string.Empty;
        public string ApellidoDifunto { get; set; } = string.Empty;
        public DateTime? FechaIngreso { get; set; }

    }
}
