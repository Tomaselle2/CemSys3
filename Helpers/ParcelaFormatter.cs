using CemSys3.Enumerables;

namespace CemSys3.Helpers
{
    public static class ParcelaFormatter
    {
        /// <summary>
        /// Obtiene la descripción formateada de una parcela según su tipo
        /// </summary>
        /// <param name="tipoParcela">Tipo de parcela: Nicho, Fosa o Panteón</param>
        /// <param name="nroParcela">Número de parcela</param>
        /// <param name="nroFila">Número de fila (solo para Nicho)</param>
        /// <param name="nombreSeccion">Nombre de la sección</param>
        /// <returns>String formateado con la descripción de la parcela</returns>
        public static string ObtenerParcela(string tipoParcela, int nroParcela, int nroFila, string nombreSeccion)
        {
            if (string.IsNullOrEmpty(tipoParcela))
                return string.Empty;

            switch (tipoParcela.ToLower())
            {
                case "nicho":
                    return $"NICHO {nroParcela} SECC {nombreSeccion.ToUpper()} FILA {nroFila}";

                case "fosa":
                    return $"FOSA {nroParcela} SECC {nombreSeccion.ToUpper()}";

                case "panteón":
                case "panteon":
                    return $"LOTE {nroParcela} SECC {nombreSeccion.ToUpper()} (panteón)";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Sobrecarga del método para cuando no se necesita fila (útil para tipos que no la usan)
        /// </summary>
        public static string ObtenerParcela(string tipoParcela, int nroParcela, string nombreSeccion)
        {
            return ObtenerParcela(tipoParcela, nroParcela, 0, nombreSeccion);
        }

        public static string ObtenerParcela(int tipoParcela, int nroParcela, int nroFila, string nombreSeccion)
        {
            if (tipoParcela == 0)
                return string.Empty;

            switch (tipoParcela)
            {
                case (int)TipoParcelaEnum.Nicho:
                    return $"NICHO {nroParcela} SECC {nombreSeccion.ToUpper()} FILA {nroFila}";

                case (int)TipoParcelaEnum.Fosa:
                    return $"FOSA {nroParcela} SECC {nombreSeccion.ToUpper()}";

                case (int)TipoParcelaEnum.Panteon:
                    return $"LOTE {nroParcela} SECC {nombreSeccion.ToUpper()} (panteón)";

                default:
                    return string.Empty;
            }
        }
    }
}

