namespace CemSys3.DTOs.Backup
{
    public class BackupEjecucionDto
    {
        public string JobName { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public DateTime FechaEjecucion { get; set; }
        public int RunStatus { get; set; }
        public string Duracion { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;

        public string EstadoTexto => RunStatus switch
        {
            0 => "Fallido",
            1 => "Exitoso",
            2 => "Reintentando",
            3 => "Cancelado",
            4 => "En progreso",
            _ => "Desconocido"
        };

        public string EstadoCss => RunStatus switch
        {
            0 => "bg-danger",
            1 => "bg-success",
            2 => "bg-warning text-dark",
            3 => "bg-secondary",
            4 => "bg-info text-dark",
            _ => "bg-secondary"
        };
    }
}
