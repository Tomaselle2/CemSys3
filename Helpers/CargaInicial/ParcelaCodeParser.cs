using System.Globalization;

namespace CemSys3.Helpers.CargaInicial
{
    public class ParcelaCodeParseResult
    {
        public bool EsValido { get; set; }
        public int NroParcela { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int NroFila { get; set; }
        public string? Error { get; set; }
    }

    /// <summary>
    /// Descompone el campo PARCELA del csv de carga inicial.
    ///
    /// Formato confirmado contra los archivos reales de Secciones/Parcelas:
    ///   [3 dígitos = NroParcela][nombre de sección, con ceros a la izquierda][2 dígitos = NroFila]
    ///
    /// Para Fosa y Panteón el NroFila se ignora siempre y se fuerza a 1 (esas parcelas
    /// siempre tienen una única fila en el sistema nuevo).
    /// </summary>
    public class ParcelaCodeParser
    {
        // Alias de nombres de sección conocidos por errores de tipeo en el sistema viejo.
        // Vacío por defecto: se completa solo si se confirma explícitamente un caso
        // (ej. { "EANTI", "EANTIG" }), para no "inventar" coincidencias no confirmadas.
        private static readonly System.Collections.Generic.Dictionary<string, string> AliasSeccion =
            new(StringComparer.OrdinalIgnoreCase)
        {
             { "EANTI", "EANTIG" },
             { "E", "EANTIG" },
             { "F", "FANTIG" },
             { "EANTG", "EANTIG" },
             { "FANTI", "FANTIG" },
             { "SECCN-2", "N-2" },
             { "NIÑOSII", "NIÑOS2" },
             { "NIÑOS(II)", "NIÑOS2" },
             { "NIÑOSI", "NIÑOS1" },
             { "C(I)", "C(1)" },
             { "(C)1", "C(1)" },
             { "C1", "C(1)" },
             { "C2", "C(2)" },
             { "C3", "C(3)" },
             { "O1", "O-1" },
             { "O2", "O-2" },
             { "O3", "O-3" },
             { "O4", "O-4" },

        };

        public static ParcelaCodeParseResult Parse(string? codigoCrudo, string tipoParcelaCsv)
        {
            var result = new ParcelaCodeParseResult();
            var codigo = codigoCrudo?.Trim();

            if (string.IsNullOrEmpty(codigo) || codigo.Length < 6)
            {
                result.Error = $"Código de parcela vacío o demasiado corto: '{codigoCrudo}'";
                return result;
            }

            var parcelaStr = codigo.Substring(0, 3);
            var filaStr = codigo.Substring(codigo.Length - 2, 2);
            var seccionRaw = codigo.Substring(3, codigo.Length - 5);
            var seccionNombre = seccionRaw.TrimStart('0');

            if (string.IsNullOrEmpty(seccionNombre))
            {
                result.Error = $"No se pudo determinar el nombre de sección a partir de '{codigo}'";
                return result;
            }

            if (AliasSeccion.TryGetValue(seccionNombre, out var alias))
            {
                seccionNombre = alias;
            }

            if (!int.TryParse(parcelaStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int nroParcela))
            {
                result.Error = $"Número de parcela inválido en '{codigo}'";
                return result;
            }

            bool esFosaOPanteon = EsFosaOPanteon(tipoParcelaCsv);

            int nroFila;
            if (esFosaOPanteon)
            {
                // Fosas y panteones siempre tienen una única fila -> se ignora lo que venga en el csv.
                nroFila = 1;
            }
            else
            {
                if (!int.TryParse(filaStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out nroFila) || nroFila < 1)
                {
                    result.Error = $"Número de fila inválido en '{codigo}'";
                    return result;
                }
            }

            result.EsValido = true;
            result.NroParcela = nroParcela;
            result.NombreSeccion = seccionNombre;
            result.NroFila = nroFila;
            return result;
        }

        private static bool EsFosaOPanteon(string tipo)
        {
            var t = tipo?.Trim().ToUpperInvariant() ?? string.Empty;
            return t == "FOSA" || t == "PANTEON" || t == "PANTEÓN";
        }

        public static int TipoParcelaIdDesdeCsv(string tipoCsv)
        {
            var t = tipoCsv?.Trim().ToUpperInvariant() ?? string.Empty;
            return t switch
            {
                "NICHO" => 1,
                "FOSA" => 2,
                "PANTEON" => 3,
                "PANTEÓN" => 3,
                _ => throw new InvalidOperationException($"Tipo de parcela desconocido en el csv: '{tipoCsv}'")
            };
        }
    }
}
