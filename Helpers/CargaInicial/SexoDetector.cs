namespace CemSys3.Helpers.CargaInicial
{
    /// <summary>
    /// Heurística offline para determinar el sexo de una persona a partir de su primer nombre.
    /// No depende de internet ni de librerías externas.
    ///
    /// Estrategia:
    ///   1. Buscar el primer nombre en listas de excepciones conocidas (nombres en español
    ///      que no siguen la regla de terminación "a"=femenino / "o"=masculino).
    ///   2. Si no está en las excepciones, usar la terminación del nombre.
    ///   3. Si no se puede determinar, devolver "otro".
    ///
    /// Las listas de excepciones son ampliables: si en la carga real aparecen nombres mal
    /// clasificados, se agregan acá y no hace falta tocar el resto del código.
    /// </summary>

    public static class SexoDetector
    {
        private static readonly HashSet<string> NombresMasculinos = new(StringComparer.OrdinalIgnoreCase)
        {
            // Terminados en consonante o "e"/"i"/"u" que son masculinos
            "JOSE","LUIS","JESUS","ANDRES","TOMAS","MATIAS","AGUSTIN","MARTIN","ADRIAN","ADRIAN",
            "RAFAEL","GABRIEL","DANIEL","MIGUEL","MANUEL","RAUL","ANIBAL","EZEQUIEL","NICOLAS",
            "SANTIAGO","IGNACIO","ANTONIO","GUILLERMO","ALFONSO","ROMUALDO","JOAQUIN","URIEL",
            "ISMAEL","EMANUEL","ABEL","NOE","AMILCAR","BALTASAR","ELIAS","ESTEBAN","ANGEL",
            "RUBEN","ISRAEL","SAMUEL","JOEL","EZEQUIAS","ISAAC","IVAN","ADAN","ABRAHAM",
            "SALOMON","MOISES","AARON","GASPAR","MELCHOR","CRISTOBAL","CARLOS","MARCOS","LUCAS",
            "AGUSTIN","VICTOR","HECTOR","OSCAR","OMAR","WALTER","NESTOR","HUGO","FEDERICO",
            "ALEJANDRO","ALBERTO","EDUARDO","ERNESTO","FERNANDO","FRANCISCO","GASTON","GERARDO",
            "GUSTAVO","HORACIO","HUMBERTO","JAVIER","JORGE","JULIO","LEANDRO","LEONARDO","LORENZO",
            "MARCELO","MARIO","MAXIMILIANO","NORBERTO","OSVALDO","PABLO","PATRICIO","PEDRO",
            "RAMON","RICARDO","RODOLFO","RODRIGO","ROGELIO","ROQUE","RUBEN","SALVADOR","SEBASTIAN",
            "SERGIO","SIMON","TEODORO","VALENTIN","VICENTE","WALDEMAR","EMILIO","ARIEL","AXEL",
            "BRUNO","CESAR","CLAUDIO","CRISTIAN","DAMIAN","DARIO","DIEGO","DOMINGO","ENRIQUE",
            "GONZALO","JEREMIAS","JUAN","LEONEL","MAXIMO","NAHUEL","NICANOR","NORMAN","OCTAVIO",
            "ORLANDO","REMIGIO","RENE","SANTOS","TOBIAS","URBANO","VALERIO","WILSON","YAMIL"
        };

        private static readonly HashSet<string> NombresFemeninos = new(StringComparer.OrdinalIgnoreCase)
        {
            // Terminados en consonante o distintas de "a" que son femeninos
            "SOLEDAD","MERCEDES","DOLORES","CONSUELO","GUADALUPE","CARMEN","PILAR","ANGELES",
            "ROSARIO","BEATRIZ","LUZ","PAZ","CRUZ","INES","ISABEL","RAQUEL","NOEMI","ABIGAIL",
            "YOLANDA","ESTHER","MIRIAM","ITATI","AIMEE","JAZMIN","BELEN","MARIEL","ANABEL",
            "MARISOL","LOURDES","VIRGINIA","ELIZABETH","MARLENE","YAEL","NOELIA","ABIGAIL",
            "ITANDEHUI","YAMIL","MARIEL","RUTH","EDITH","MARGARET","MARGOT","NURIA","YAMILA",
            "ANAHI","ANALIA","ANDREA","AZUL","BELKIS","CARIDAD","DALILA","DAYANA","DEBORA",
            "ELIZABET","ESTER","EVELYN","GISEL","GRISEL","HAYDEE","IRIS","ITATÍ","JAQUELIN",
            "JOSEFIN","KARINA","LEDESMA","LILIAN","MABEL","MADELEINE","MAGALI","MARLEN",
            "MERI","MIREYA","NADIR","NANCY","NAYELI","NOOR","RAKEL","SHEYLA","VIVIAN","YASMIN",
            "YESICA","YAMILET","ZULEMA","ZORAIDA","AZUCENA","ROSMERI","MAITE"
        };

        public static string Detectar(string? nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                return "otro";

            var primerNombre = nombreCompleto
                .Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(primerNombre))
                return "otro";

            if (NombresMasculinos.Contains(primerNombre))
                return "masculino";

            if (NombresFemeninos.Contains(primerNombre))
                return "femenino";

            var upper = primerNombre.ToUpperInvariant();

            if (upper.EndsWith("A"))
                return "femenino";

            if (upper.EndsWith("O") || upper.EndsWith("E"))
                return "masculino";

            return "otro";
        }

        /// <summary>
        /// Si ya viene el sexo cargado en el CSV ("M"/"F"), se usa ese valor directamente
        /// (más confiable que la heurística). Solo se recurre a Detectar() si viene vacío.
        /// </summary>
        public static string DesdeCsvOHeuristica(string? sexoCsv, string? nombreCompleto)
        {
            var valor = sexoCsv?.Trim().ToUpperInvariant();

            return valor switch
            {
                "M" => "masculino",
                "F" => "femenino",
                _ => Detectar(nombreCompleto)
            };
        }
    }
}
