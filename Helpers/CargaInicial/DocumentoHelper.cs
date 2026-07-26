using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CemSys3.Helpers.CargaInicial
{

    public class NombreApellido
    {
        public string Apellido { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }

    public class DocumentoHelper
    {
        // El sistema viejo exporta "APELLIDO     NOMBRE" separados por 5 o más espacios.
        // Si no hay un separador así de grande, se toma todo como apellido.
        private static readonly Regex SeparadorRegex = new(@"\s{5,}", RegexOptions.Compiled);

        public static NombreApellido SepararNombreApellido(string? crudo)
        {
            var resultado = new NombreApellido();

            if (string.IsNullOrWhiteSpace(crudo))
                return resultado;

            var partes = SeparadorRegex.Split(crudo.Trim());

            resultado.Apellido = partes.Length > 0 ? partes[0].Trim().ToLowerInvariant() : string.Empty;
            resultado.Nombre = partes.Length > 1 ? partes[1].Trim().ToLowerInvariant() : string.Empty;

            return resultado;
        }

        /// <summary>
        /// Extrae el DNI a partir del tipo de documento y el número crudo.
        /// - DNI / D / T -> se usa el número tal cual viene.
        /// - CUIT -> formato XX-DNI(8 dígitos)-Y => se descartan los 2 primeros dígitos
        ///   y el último, quedando el DNI de 8 dígitos (puede tener cero a la izquierda).
        /// </summary>
        public static string? ExtraerDni(string? tipoDocumento, string? numeroCrudo)
        {
            if (string.IsNullOrWhiteSpace(numeroCrudo))
                return null;

            var numero = new string(numeroCrudo.Where(char.IsDigit).ToArray());
            var tipo = tipoDocumento?.Trim().ToUpperInvariant() ?? string.Empty;

            if (tipo == "CUIT")
            {
                if (numero.Length <= 3)
                {
                    // No hay suficientes dígitos para extraer nada razonable.
                    return numero;
                }

                return numero.Substring(2, numero.Length - 3);
            }

            // DNI, D, T u otro valor -> se usa tal cual.
            return numero;
        }
    }
}
