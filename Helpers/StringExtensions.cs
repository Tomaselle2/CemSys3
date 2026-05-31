namespace CemSys3.Helpers
{
    public static class StringExtensions
    {
        public static string FormatearDni(this string dni)
        {
            if (string.IsNullOrEmpty(dni))
                return string.Empty;

            // Eliminar cualquier punto o guión existente
            string dniLimpio = dni.Replace(".", "").Replace("-", "");

            if (dniLimpio.Length <= 3)
                return dniLimpio;

            // Formatear con puntos cada 3 dígitos de derecha a izquierda
            for (int i = dniLimpio.Length - 3; i > 0; i -= 3)
            {
                dniLimpio = dniLimpio.Insert(i, ".");
            }

            return dniLimpio;
        }
    }
}
