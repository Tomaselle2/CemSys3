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
        // El sistema viejo exporta "APELLIDO<espacios>NOMBRE" sin un ancho fijo consistente:
        // - Cuando el valor sale del campo Apellido de ancho fijo (CHAR(31)) del sistema viejo,
        //   el relleno son muchos espacios (10, 20, hasta 27).
        // - Cuando alguien lo tipeó a mano, el "separador" puede ser de apenas 2 espacios
        //   (ej: "SOLA  PEDRO TOMAS", o "BARRERA ROLDAN  LIUIS ALBERTO" con apellido compuesto).
        // Con el umbral anterior (5+) estos casos de 2-4 espacios NO matcheaban y quedaban
        // enteros en Apellido. Se baja a 2+, que es el mínimo real observado en el csv y no
        // aparece nunca como espaciado "accidental" dentro de un nombre o apellido (esos van
        // siempre con un solo espacio).
        //
        // Si no hay ni un separador de 2+ espacios, se sigue tomando todo como apellido
        // (mismo criterio que antes): no hay forma confiable de saber dónde cortar cuando
        // todo el valor tiene espacios simples (puede ser apellido compuesto: "MOSCOSO RAMALLO
        // LUIS MARIA", o nombre compuesto: "CONCI NILDA MARIA").
        private static readonly Regex SeparadorRegex = new(@"\s{2,}", RegexOptions.Compiled);

        public static NombreApellido SepararNombreApellido(string? crudo)
        {
            var resultado = new NombreApellido();

            if (string.IsNullOrWhiteSpace(crudo))
                return resultado;

            var valor = crudo.Trim();

            // Match en vez de Split: si por algún motivo hubiera más de un tramo de espacios
            // grandes en el mismo valor, todo lo que viene después del PRIMERO se conserva
            // como Nombre en vez de perderse (Split con esta regex descartaría lo que sobra
            // después del segundo tramo).
            var match = SeparadorRegex.Match(valor);

            if (match.Success)
            {
                var apellido = valor[..match.Index].Trim();
                var nombre = valor[(match.Index + match.Length)..].Trim();

                if (apellido.Length > 0 && nombre.Length > 0)
                {
                    resultado.Apellido = apellido.ToLowerInvariant();
                    resultado.Nombre = nombre.ToLowerInvariant();
                    return resultado;
                }
            }

            // Sin separador confiable: todo como apellido (igual que antes).
            resultado.Apellido = valor.ToLowerInvariant();
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