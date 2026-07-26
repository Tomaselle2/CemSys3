namespace CemSys3.DTOs.CargaInicial
{
    public class ResultadoFilaCarga
    {
        public CargaInicialCsvRow Fila { get; set; } = null!;
        public bool Exito { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public int? TramiteConcesionId { get; set; }
        public int? DifuntoPersonaId { get; set; }
        public int? TitularPersonaId { get; set; }
    }
}
