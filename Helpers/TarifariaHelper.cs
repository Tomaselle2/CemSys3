namespace CemSys3.Helpers
{
    public static class TarifariaHelper
    {
        public static decimal Redondear(decimal valor, int decimales)
        {
            if (decimales >= 0)
                return Math.Round(valor, decimales, MidpointRounding.AwayFromZero);
            decimal factor = (decimal)Math.Pow(10, -decimales);
            return Math.Round(valor / factor, MidpointRounding.AwayFromZero) * factor;
        }
    }
}
