using CsvHelper.Configuration.Attributes;

namespace CemSys3.DTOs.CargaInicial
{
    /// <summary>
    /// Mapea 1:1 el encabezado del csv de carga inicial del cementerio.
    /// Todo se lee como string: la conversión/validación de tipos (fechas, números)
    /// se hace explícitamente en el servicio para poder reportar errores por campo.
    /// </summary>
    public class CargaInicialCsvRow
    {
        [Name("Nro.")]
        public string? Nro { get; set; }

        [Name("PARCELA")]
        public string? Parcela { get; set; }

        [Name("CALLE")]
        public string? Calle { get; set; } // se ignora siempre, se mantiene solo para el archivo de salida

        [Name("CONCESION")]
        public string? Concesion { get; set; }

        [Name("TIPO")]
        public string? Tipo { get; set; }

        [Name("CATEGORIA")]
        public string? Categoria { get; set; } // se ignora siempre

        [Name("FECHA INICIO")]
        public string? FechaInicio { get; set; }

        [Name("FECHA VTO.")]
        public string? FechaVto { get; set; }

        [Name("ACTIVO")]
        public string? Activo { get; set; }

        [Name("IMPRIME")]
        public string? Imprime { get; set; } // se ignora siempre

        [Name("TIPO_PLAN")]
        public string? TipoPlan { get; set; } // se ignora siempre

        [Name("ENCARGADO_PAGO")]
        public string? EncargadoPago { get; set; }

        [Name("TIPO_DOC_ENCARGADO_PAGO")]
        public string? TipoDocEncargadoPago { get; set; }

        [Name("DOCUMENTO_ENCARGADO_PAGO")]
        public string? DocumentoEncargadoPago { get; set; }

        [Name("Num_celular")]
        public string? NumCelular { get; set; }

        [Name("SEXO_ENCARGADO")]
        public string? SexoEncargadoCsv { get; set; }

        [Name("MAIL_ENCARGADO")]
        public string? MailEncargado { get; set; }

        [Name("FALLECIDO")]
        public string? Fallecido { get; set; }

        [Name("TIPO_DOCUMENTO_FALLECIDO")]
        public string? TipoDocumentoFallecido { get; set; }

        [Name("DOCUMENTO_FALLECIDO")]
        public string? DocumentoFallecido { get; set; }

        [Name("SEXO_FALLECIDO")]
        public string? SexoFallecidoCsv { get; set; }

        [Name("FECHA_FALLECIMIENTO")]
        public string? FechaFallecimiento { get; set; }

        /// <summary>Número de línea real dentro del archivo (para trazabilidad en el log de errores).</summary>
        [Ignore]
        public int NumeroFilaOriginal { get; set; }
    }
}

