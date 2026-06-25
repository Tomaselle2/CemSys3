namespace CemSys3.DTOs.Tarifaria
{
    public class PdfPreciosNichosDTO
    {
        // Porcentaje de fondo como decimal (ej: 0.05 = 5 %)
        public decimal PorcentajeFondo { get; set; }

        // Grupos para la sección LOCAL de nichos
        public List<GrupoSeccionNichoPdfDTO> GruposNichosLocales { get; set; } = new();

        // Grupos para la sección OTRAS JURISDICCIONES de nichos
        public List<GrupoSeccionNichoPdfDTO> GruposNichosOtrasJurisdicciones { get; set; } = new();

        // Precios de fosas (local)
        public List<PrecioFosaPdfDTO> FosasLocales { get; set; } = new();

        // Precios de fosas (otras jurisdicciones)
        public List<PrecioFosaPdfDTO> FosasOtrasJurisdicciones { get; set; } = new();
    }
}
