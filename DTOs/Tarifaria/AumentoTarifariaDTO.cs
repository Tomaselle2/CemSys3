namespace CemSys3.DTOs.Tarifaria
{
    public class AumentoTarifariaDTO
    {
        public decimal Porcentaje { get; set; }  // Ej: 35.5 para 35.5%
        public int Decimales { get; set; }       // 0=pesos, 2=centavos, -2=centenas
    }
}
