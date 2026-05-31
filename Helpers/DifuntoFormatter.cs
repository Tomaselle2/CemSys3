using CemSys3.DTOs.Persona;

namespace CemSys3.Helpers
{
    public static class DifuntoFormatter
    {
        /// <summary>
        /// Formatea una lista de difuntos en un string legible
        /// </summary>
        /// <param name="difuntos">Lista de difuntos</param>
        /// <returns>String formateado: "APELLIDO NOMBRE", "APELLIDO1 NOMBRE1 y APELLIDO2 NOMBRE2", o "APELLIDO1 NOMBRE1, APELLIDO2 NOMBRE2 y APELLIDO3 NOMBRE3"</returns>
        public static string FormatearDifuntos(IEnumerable<DifuntoContratoDTO> difuntos)
        {
            if (difuntos == null || !difuntos.Any())
                return string.Empty;

            var nombresDifuntos = difuntos.Select(d => $"{d.Apellido.ToUpper()} {d.Nombre.ToUpper()}");

            if (nombresDifuntos.Count() == 1)
                return nombresDifuntos.First();
            else if (nombresDifuntos.Count() == 2)
                return string.Join(" y ", nombresDifuntos);
            else
                return string.Join(", ", nombresDifuntos.Take(nombresDifuntos.Count() - 1)) + " y " + nombresDifuntos.Last();
        }
        
    }

}
