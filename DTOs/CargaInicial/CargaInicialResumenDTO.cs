namespace CemSys3.DTOs.CargaInicial
{
    public class CargaInicialResumenDTO
    {

        public int TotalFilas { get; set; }
        public int TotalGrupos { get; set; }
        public int TotalExitosas { get; set; }
        public int TotalErrores { get; set; }
        public bool ModoPrueba { get; set; }

        /// <summary>CSV con las filas que se cargaron correctamente (mismas columnas + Ids generados).</summary>
        public byte[] ArchivoExitososCsv { get; set; } = System.Array.Empty<byte>();

        /// <summary>CSV con las filas que fallaron (mismas columnas + columna "Motivo").</summary>
        public byte[] ArchivoErroresCsv { get; set; } = System.Array.Empty<byte>();
    }
}
